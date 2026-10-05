using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InvoicePrinting.Models;

public enum ContentKind
{
    Image,
    PdfPage
}

public sealed class ContentItem : INotifyPropertyChanged
{
    private readonly Dictionary<int, BitmapSource> _rotatedPreviews = new();
    private int _manualRotation;

    public ContentItem(
        string sourcePath,
        ContentKind kind,
        int? pageNumber,
        double sourceWidth,
        double sourceHeight,
        BitmapSource preview)
    {
        SourcePath = sourcePath;
        Kind = kind;
        PageNumber = pageNumber;
        SourceWidth = sourceWidth;
        SourceHeight = sourceHeight;
        Preview = preview;
        _rotatedPreviews[0] = preview;
    }

    public string Id { get; } = Guid.NewGuid().ToString("N");
    public string SourcePath { get; }
    public string FileName => Path.GetFileName(SourcePath);
    public ContentKind Kind { get; }
    public int? PageNumber { get; }
    public double SourceWidth { get; }
    public double SourceHeight { get; }
    public BitmapSource Preview { get; }

    public BitmapSource Thumbnail => GetPreview(ManualRotation);

    public string DisplayDetails => $"{(Kind == ContentKind.PdfPage ? $"PDF · 第 {PageNumber} 页" : "图片")} · " +
        (HasManualRotation ? $"旋转 {ManualRotation}°" : OrientationText());

    public int ManualRotation
    {
        get => _manualRotation;
        private set
        {
            _manualRotation = value % 360;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Thumbnail));
            OnPropertyChanged(nameof(DisplayDetails));
        }
    }

    public bool HasManualRotation => ManualRotation != 0;

    public string Description => Kind == ContentKind.PdfPage
        ? $"{FileName} · 第 {PageNumber} 页 · {OrientationText()}"
        : $"{FileName} · {OrientationText()}";

    public void RotateClockwise()
    {
        ManualRotation = (ManualRotation + 90) % 360;
    }

    public void ResetRotation()
    {
        ManualRotation = 0;
    }

    public BitmapSource GetPreview(int rotation)
    {
        rotation = ((rotation % 360) + 360) % 360;
        if (_rotatedPreviews.TryGetValue(rotation, out var cached))
        {
            return cached;
        }

        var transformed = new TransformedBitmap(Preview, new RotateTransform(rotation));
        transformed.Freeze();
        _rotatedPreviews[rotation] = transformed;
        return transformed;
    }

    private string OrientationText()
    {
        var width = ManualRotation % 180 == 0 ? SourceWidth : SourceHeight;
        var height = ManualRotation % 180 == 0 ? SourceHeight : SourceWidth;
        return width >= height ? "横向" : "纵向";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

