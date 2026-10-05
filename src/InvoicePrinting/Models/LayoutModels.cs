using System.Windows;
using System.Windows.Media.Imaging;
using WpfSize = System.Windows.Size;

namespace InvoicePrinting.Models;

public enum PageOrientation
{
    Auto,
    Portrait,
    Landscape
}

public sealed record LayoutOptions(
    int ItemsPerSheet,
    PageOrientation Orientation,
    double MarginMm = 5,
    double GutterMm = 4);

public sealed record LayoutPlacement(
    ContentItem Item,
    Rect CellRect,
    Rect ContentRect,
    int Rotation,
    BitmapSource Image);

public static class LayoutEngine
{
    public const double A4WidthMm = 210;
    public const double A4HeightMm = 297;
    public const double DipPerInch = 96;
    public const double MmPerInch = 25.4;

    public static WpfSize GetPageSize(LayoutOptions options, double shortSideDip = 420)
    {
        var portrait = new WpfSize(shortSideDip, shortSideDip * A4HeightMm / A4WidthMm);
        var landscape = new WpfSize(portrait.Height, portrait.Width);
        return options.Orientation == PageOrientation.Landscape ? landscape : portrait;
    }

    public static int GetPageCount(int itemCount, int itemsPerSheet)
        => itemCount == 0 ? 0 : (itemCount + itemsPerSheet - 1) / itemsPerSheet;

    public static IReadOnlyList<LayoutPlacement> Calculate(
        IReadOnlyList<ContentItem> items,
        LayoutOptions options,
        int pageIndex,
        WpfSize pageSize)
    {
        if (options.ItemsPerSheet is not (1 or 2 or 4))
        {
            throw new ArgumentOutOfRangeException(nameof(options.ItemsPerSheet));
        }

        var (columns, rows) = GetGrid(options);
        var marginX = MmToDip(options.MarginMm, A4WidthMm, pageSize.Width);
        var marginY = MmToDip(options.MarginMm, A4HeightMm, pageSize.Height);
        var gutterX = MmToDip(options.GutterMm, A4WidthMm, pageSize.Width);
        var gutterY = MmToDip(options.GutterMm, A4HeightMm, pageSize.Height);
        var cellWidth = (pageSize.Width - (marginX * 2) - gutterX * (columns - 1)) / columns;
        var cellHeight = (pageSize.Height - (marginY * 2) - gutterY * (rows - 1)) / rows;

        var pageItems = items
            .Skip(pageIndex * options.ItemsPerSheet)
            .Take(options.ItemsPerSheet)
            .ToArray();

        var placements = new List<LayoutPlacement>(pageItems.Length);
        for (var index = 0; index < pageItems.Length; index++)
        {
            var item = pageItems[index];
            var column = index % columns;
            var row = index / columns;
            var cell = new Rect(
                marginX + column * (cellWidth + gutterX),
                marginY + row * (cellHeight + gutterY),
                cellWidth,
                cellHeight);

            var rotation = item.HasManualRotation
                ? item.ManualRotation
                : SelectAutomaticRotation(item, cell.Size);

            var sourceWidth = rotation % 180 == 0 ? item.SourceWidth : item.SourceHeight;
            var sourceHeight = rotation % 180 == 0 ? item.SourceHeight : item.SourceWidth;
            var scale = Math.Min(cell.Width / sourceWidth, cell.Height / sourceHeight);
            var contentWidth = sourceWidth * scale;
            var contentHeight = sourceHeight * scale;
            var content = new Rect(
                cell.Left + (cell.Width - contentWidth) / 2,
                cell.Top + (cell.Height - contentHeight) / 2,
                contentWidth,
                contentHeight);

            placements.Add(new LayoutPlacement(item, cell, content, rotation, item.GetPreview(rotation)));
        }

        return placements;
    }

    private static int SelectAutomaticRotation(ContentItem item, WpfSize cellSize)
    {
        var contentIsLandscape = item.SourceWidth > item.SourceHeight;
        var cellIsLandscape = cellSize.Width > cellSize.Height;
        return contentIsLandscape == cellIsLandscape ? 0 : 90;
    }

    private static (int Columns, int Rows) GetGrid(LayoutOptions options)
    {
        return options.ItemsPerSheet switch
        {
            1 => (1, 1),
            2 when options.Orientation == PageOrientation.Landscape => (2, 1),
            2 => (1, 2),
            4 => (2, 2),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static double MmToDip(double mm, double referenceMm, double referenceDip)
        => mm / referenceMm * referenceDip;
}

