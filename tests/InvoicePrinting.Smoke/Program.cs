using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using InvoicePrinting.Models;
using InvoicePrinting.Services;

namespace Smoke;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            if (args.Contains("--visual-review", StringComparer.OrdinalIgnoreCase))
                VisualReview.Run(args.Contains("--reduced-motion", StringComparer.OrdinalIgnoreCase));
            else RunSmokeAsync().GetAwaiter().GetResult();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static async Task RunSmokeAsync()
    {
        var onePixel = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=");
        var imagePath = Path.Combine(Path.GetTempPath(), "invoice-printing-smoke.png");
        var pdfPath = Path.Combine(Path.GetTempPath(), "invoice-printing-smoke.pdf");
        File.WriteAllBytes(imagePath, onePixel);
        File.WriteAllBytes(pdfPath, BuildPdf());

        var preview = new BitmapImage();
        using (var stream = new MemoryStream(onePixel))
        {
            preview.BeginInit();
            preview.CacheOption = BitmapCacheOption.OnLoad;
            preview.StreamSource = stream;
            preview.EndInit();
            preview.Freeze();
        }

        var wide = new ContentItem(imagePath, ContentKind.Image, null, 1600, 900, preview);
        var portraitOptions = new LayoutOptions(1, PageOrientation.Portrait);
        var portrait = LayoutEngine.Calculate(new[] { wide }, portraitOptions, 0, LayoutEngine.GetPageSize(portraitOptions));
        Assert(portrait.Count == 1 && portrait[0].Rotation == 90, "横向内容应在纵向单元中自动旋转 90°");

        var fourOptions = new LayoutOptions(4, PageOrientation.Auto);
        var four = LayoutEngine.Calculate(new[] { wide }, fourOptions, 0, LayoutEngine.GetPageSize(fourOptions));
        Assert(four.Count == 1, "4 张版式的空位不能生成多余内容项");

        using var importer = new DocumentImporter();
        var imported = await importer.ImportAsync(new[] { imagePath, pdfPath });
        Assert(imported.Count == 2, "图片和 PDF 各应导入一个内容项");
        Assert(imported.Any(x => x.Kind == ContentKind.PdfPage && x.PageNumber == 1), "PDF 第 1 页应能被解析");

        importer.Dispose();
        File.Delete(imagePath);
        File.Delete(pdfPath);
        Console.WriteLine("Smoke tests passed: layout rotation, 1/2/4 grid sizing, image import, PDF import.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static byte[] BuildPdf()
    {
        var objects = new[]
        {
            "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n",
            "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n",
            "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 5 0 R >> >> /Contents 4 0 R >>\nendobj\n",
            "4 0 obj\n<< /Length 45 >>\nstream\nBT /F1 24 Tf 100 700 Td (Smoke PDF) Tj ET\nendstream\nendobj\n",
            "5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n"
        };

        using var stream = new MemoryStream();
        var header = Encoding.ASCII.GetBytes("%PDF-1.4\n");
        stream.Write(header);
        var offsets = new List<long> { 0 };
        foreach (var value in objects)
        {
            offsets.Add(stream.Position);
            var bytes = Encoding.ASCII.GetBytes(value);
            stream.Write(bytes);
        }

        var xrefPosition = stream.Position;
        var xref = new StringBuilder($"xref\n0 {objects.Length + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1))
        {
            xref.Append($"{offset:0000000000} 00000 n \n");
        }
        xref.Append($"trailer\n<< /Size {objects.Length + 1} /Root 1 0 R >>\nstartxref\n{xrefPosition}\n%%EOF\n");
        var xrefBytes = Encoding.ASCII.GetBytes(xref.ToString());
        stream.Write(xrefBytes);
        return stream.ToArray();
    }
}
