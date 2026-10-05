using System.IO;
using PdfiumViewer;

namespace InvoicePrinting.Services;

internal static class PdfiumBootstrapper
{
    private const string ResourceName = "InvoicePrinting.Native.pdfium.x64.dll";
    private static readonly object SyncRoot = new();
    private static string? _nativePath;
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            PdfiumResolver.Resolve += ResolvePdfium;
            _initialized = true;
        }
    }

    private static void ResolvePdfium(object? sender, PdfiumResolveEventArgs e)
    {
        e.PdfiumFileName = GetNativePath();
    }

    private static string GetNativePath()
    {
        lock (SyncRoot)
        {
            if (_nativePath is not null)
            {
                return _nativePath;
            }

            var targetDirectory = Path.Combine(
                Path.GetTempPath(),
                "InvoicePrinting",
                "pdfium",
                "x64");
            Directory.CreateDirectory(targetDirectory);
            var targetPath = Path.Combine(targetDirectory, "pdfium.dll");

            var assembly = typeof(PdfiumBootstrapper).Assembly;
            using var source = assembly.GetManifestResourceStream(ResourceName)
                ?? throw new InvalidOperationException("内置 PDF 渲染组件丢失，请重新发布程序。");

            if (!File.Exists(targetPath) || new FileInfo(targetPath).Length != source.Length)
            {
                var temporaryPath = targetPath + ".tmp-" + Guid.NewGuid().ToString("N");
                try
                {
                    using (var target = File.Create(temporaryPath))
                    {
                        source.CopyTo(target);
                    }

                    File.Move(temporaryPath, targetPath, true);
                }
                finally
                {
                    if (File.Exists(temporaryPath))
                    {
                        File.Delete(temporaryPath);
                    }
                }
            }

            _nativePath = targetPath;
            return _nativePath;
        }
    }
}
