using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using PdfiumViewer;
using InvoicePrinting.Models;

namespace InvoicePrinting.Services;

public sealed class DocumentImporter : IDisposable
{
    private readonly List<IDisposable> _pdfDocuments = new();

    public DocumentImporter()
    {
        PdfiumBootstrapper.Initialize();
    }

    public Task<IReadOnlyList<ContentItem>> ImportAsync(
        IEnumerable<string> paths,
        CancellationToken cancellationToken = default)
    {
        var pathArray = paths.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        return Task.Run(() => Import(pathArray, cancellationToken), cancellationToken);
    }

    private IReadOnlyList<ContentItem> Import(
        IReadOnlyList<string> paths,
        CancellationToken cancellationToken)
    {
        var result = new List<ContentItem>();
        foreach (var path in paths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var extension = Path.GetExtension(path).ToLowerInvariant();
            if (extension == ".pdf")
            {
                ImportPdf(path, result, cancellationToken);
            }
            else if (IsImage(extension))
            {
                result.Add(ImportImage(path));
            }
            else
            {
                throw new NotSupportedException($"不支持的文件格式：{Path.GetFileName(path)}");
            }
        }

        return result;
    }

    private void ImportPdf(string path, ICollection<ContentItem> result, CancellationToken cancellationToken)
    {
        var document = PdfDocument.Load(path);
        _pdfDocuments.Add(document);

        for (var page = 0; page < document.PageCount; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pageSize = document.PageSizes[page];
            using var rendered = document.Render(
                page,
                144,
                144,
                PdfRenderFlags.ForPrinting | PdfRenderFlags.Annotations | PdfRenderFlags.CorrectFromDpi);
            var preview = BitmapToSource(rendered);
            result.Add(new ContentItem(
                path,
                ContentKind.PdfPage,
                page + 1,
                pageSize.Width,
                pageSize.Height,
                preview));
        }
    }

    private static ContentItem ImportImage(string path)
    {
        var preview = LoadImage(path);
        return new ContentItem(
            path,
            ContentKind.Image,
            null,
            preview.PixelWidth,
            preview.PixelHeight,
            preview);
    }

    private static BitmapSource LoadImage(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(path, UriKind.Absolute);
        bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    private static BitmapSource BitmapToSource(Image bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }

    private static bool IsImage(string extension)
        => extension is ".png" or ".jpg" or ".jpeg" or ".bmp" or ".tif" or ".tiff" or ".webp";

    public void Dispose()
    {
        foreach (var document in _pdfDocuments)
        {
            document.Dispose();
        }

        _pdfDocuments.Clear();
    }
}


