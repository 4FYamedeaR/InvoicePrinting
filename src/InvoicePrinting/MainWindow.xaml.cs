using System.Collections.ObjectModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using InvoicePrinting.Models;
using InvoicePrinting.Services;
using Microsoft.Win32;
using WpfDragEventArgs = System.Windows.DragEventArgs;
using WpfKeyEventArgs = System.Windows.Input.KeyEventArgs;
using ModelPageOrientation = InvoicePrinting.Models.PageOrientation;

namespace InvoicePrinting;

public partial class MainWindow : Window
{
    private readonly DocumentImporter _importer = new();
    private int _itemsPerSheet = 1;
    private ModelPageOrientation _pageOrientation = ModelPageOrientation.Auto;
    private bool _isRefreshing;
    private int _previewPage;
    private bool _viewReady;
    private double _previewScale = 1.0;
    private bool _previewScaleUserSet;
    private bool _fitPreviewScheduled;
    private bool _closeAfterImport;
    private Brush? _dropZoneBackground;
    private Brush? _dropZoneBorderBrush;
    private bool _dropHighlighted;
    private readonly Brush _dropHighlightBackground = new SolidColorBrush(Color.FromRgb(224, 237, 207));
    private readonly Brush _dropHighlightBorderBrush = new SolidColorBrush(Color.FromRgb(57, 97, 62));

    private const double MinPreviewScale = 0.2;
    private const double MaxPreviewScale = 3.0;
    private const double PreviewScaleStep = 0.1;

    public ObservableCollection<ContentItem> Items { get; } = new();
    public bool ImportBusy { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        _dropZoneBackground = DropZone.Background;
        _dropZoneBorderBrush = DropZone.BorderBrush;
        ApplyPreviewScale();
        _viewReady = true;
        DataContext = this;
        Items.CollectionChanged += (_, _) =>
        {
            _previewPage = Math.Min(_previewPage, Math.Max(0, PageCount - 1));
            if (!ImportBusy)
            {
                RefreshPreview();
            }
        };
        UpdateResponsiveLayout();
        RefreshPreview();
    }

    private int PageCount => LayoutEngine.GetPageCount(Items.Count, _itemsPerSheet);

    private LayoutOptions CurrentOptions => new(
        _itemsPerSheet,
        _pageOrientation,
        MarginMm: 5,
        GutterMm: 4);

    private async void AddFiles_Click(object sender, RoutedEventArgs e)
    {
        if (ImportBusy)
        {
            return;
        }

        var dialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "支持的文件|*.pdf;*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff;*.webp|所有文件|*.*",
            Title = "选择发票或图片"
        };
        if (dialog.ShowDialog(this) == true)
        {
            await AddFilesAsync(dialog.FileNames);
        }
    }

    private async void Window_Drop(object sender, WpfDragEventArgs e)
    {
        SetDropHighlight(false);
        e.Handled = true;
        if (ImportBusy || !e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return;
        }

        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
        {
            await AddFilesAsync(paths);
        }
    }

    private void Window_DragOver(object sender, WpfDragEventArgs e)
    {
        var canImport = !ImportBusy && e.Data.GetDataPresent(DataFormats.FileDrop);
        e.Effects = canImport
            ? DragDropEffects.Copy
            : DragDropEffects.None;
        SetDropHighlight(canImport);
        e.Handled = true;
    }

    private void Window_DragLeave(object sender, WpfDragEventArgs e)
    {
        SetDropHighlight(false);
    }

    private void SetDropHighlight(bool highlighted)
    {
        if (!_viewReady || _dropHighlighted == highlighted)
        {
            return;
        }

        _dropHighlighted = highlighted;
        DropZone.Background = highlighted
            ? _dropHighlightBackground
            : _dropZoneBackground;
        DropZone.BorderBrush = highlighted
            ? _dropHighlightBorderBrush
            : _dropZoneBorderBrush;
    }

    private async Task AddFilesAsync(IEnumerable<string> paths)
    {
        if (ImportBusy)
        {
            return;
        }

        var validPaths = paths.Where(File.Exists).ToArray();
        if (validPaths.Length == 0)
        {
            return;
        }

        SetBusy(true, $"正在解析 {validPaths.Length} 个文件…");
        try
        {
            var imported = await _importer.ImportAsync(validPaths);
            foreach (var item in imported)
            {
                Items.Add(item);
            }

            StatusText.Text = $"已添加 {imported.Count} 个内容项 · 可以继续添加或开始打印";
        }
        catch (Exception ex)
        {
            StatusText.Text = "文件解析失败";
            if (!_closeAfterImport)
            {
                MessageBox.Show(this, ex.Message, "无法添加文件", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        finally
        {
            SetBusy(false);
            RefreshPreview();
            if (_closeAfterImport)
            {
                Close();
            }
        }
    }

    private void LayoutOption_Click(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string tag } && int.TryParse(tag, out var value))
        {
            _itemsPerSheet = value;
            _previewPage = 0;
            RefreshPreview();
        }
    }

    private void OrientationCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OrientationCombo.SelectedItem is ComboBoxItem { Tag: string tag } &&
            Enum.TryParse<ModelPageOrientation>(tag, out var orientation))
        {
            _pageOrientation = orientation;
            RefreshPreview();
        }
    }

    private void CropMarks_Changed(object sender, RoutedEventArgs e) => RefreshPreview();

    private void RotateItem_Click(object sender, RoutedEventArgs e)
    {
        if (!ImportBusy && sender is FrameworkElement { DataContext: ContentItem item })
        {
            item.RotateClockwise();
            RefreshPreview();
            StatusText.Text = $"已旋转 {item.FileName} · {item.ManualRotation}°";
        }
    }

    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    {
        if (!ImportBusy && sender is FrameworkElement { DataContext: ContentItem item })
        {
            Items.Remove(item);
            StatusText.Text = "已删除 1 个内容项";
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        if (ImportBusy || Items.Count == 0)
        {
            return;
        }

        Items.Clear();
        _importer.Dispose();
        StatusText.Text = "列表已清空";
    }

    private void PreviousPage_Click(object sender, RoutedEventArgs e)
    {
        if (_previewPage > 0)
        {
            _previewPage--;
            RefreshPreview();
            StatusText.Text = $"正在预览第 {_previewPage + 1} / {PageCount} 页";
        }
    }

    private void NextPage_Click(object sender, RoutedEventArgs e)
    {
        if (_previewPage + 1 < PageCount)
        {
            _previewPage++;
            RefreshPreview();
            StatusText.Text = $"正在预览第 {_previewPage + 1} / {PageCount} 页";
        }
    }

    private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ItemsList.SelectedItem is ContentItem selected)
        {
            var index = Items.IndexOf(selected);
            _previewPage = index / _itemsPerSheet;
            RefreshPreview();
            if (!ImportBusy)
            {
                StatusText.Text = $"已选中 {selected.FileName} · 第 {_previewPage + 1} 页";
            }
        }
    }

    private void PreviewScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!_previewScaleUserSet)
        {
            ScheduleFitPreviewToViewport();
        }
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_viewReady)
        {
            UpdateResponsiveLayout();
        }
    }

    private void UpdateResponsiveLayout()
    {
        var contentWidth = (Content as FrameworkElement)?.ActualWidth ?? 0;
        var windowWidth = contentWidth > 0 ? contentWidth : Width;
        SidebarColumn.Width = new GridLength(windowWidth < 1040 ? 300 : 340);
        SidebarGapColumn.Width = new GridLength(windowWidth < 1040 ? 18 : 26);
        HeaderSteps.Visibility = windowWidth < 960 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateResponsiveLayout();
        ScheduleFitPreviewToViewport();
        if (SystemParameters.ClientAreaAnimation)
        {
            WorkspaceGrid.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(280),
                FillBehavior = FillBehavior.Stop,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            });
        }
    }

    private void FitPreview_Click(object sender, RoutedEventArgs e)
    {
        _previewScaleUserSet = false;
        FitPreviewToViewport();
    }

    private void ZoomOut_Click(object sender, RoutedEventArgs e)
        => ChangePreviewScale(-PreviewScaleStep);

    private void ZoomIn_Click(object sender, RoutedEventArgs e)
        => ChangePreviewScale(PreviewScaleStep);

    private void ChangePreviewScale(double delta)
    {
        var nextScale = Math.Clamp(_previewScale + delta, MinPreviewScale, MaxPreviewScale);
        if (Math.Abs(nextScale - _previewScale) < 0.001)
        {
            return;
        }

        SetPreviewScaleAtPoint(nextScale, new Point(
            PreviewScrollViewer.ViewportWidth / 2,
            PreviewScrollViewer.ViewportHeight / 2));
    }

    private void PreviewScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not ScrollViewer viewer || e.Delta == 0)
        {
            return;
        }

        var nextScale = Math.Clamp(
            _previewScale + (e.Delta > 0 ? PreviewScaleStep : -PreviewScaleStep),
            MinPreviewScale,
            MaxPreviewScale);
        if (Math.Abs(nextScale - _previewScale) < 0.001)
        {
            e.Handled = true;
            return;
        }

        SetPreviewScaleAtPoint(nextScale, e.GetPosition(viewer));
        e.Handled = true;
    }

    private void SetPreviewScaleAtPoint(double scale, Point anchor)
    {
        // Translate through the visual tree so centered paper and its margin do not move the zoom anchor.
        var canvasPoint = PreviewScrollViewer.TranslatePoint(anchor, PreviewCanvas);
        _previewScale = scale;
        _previewScaleUserSet = true;
        ApplyPreviewScale();
        PreviewScrollViewer.UpdateLayout();
        var movedAnchor = PreviewCanvas.TranslatePoint(canvasPoint, PreviewScrollViewer);
        PreviewScrollViewer.ScrollToHorizontalOffset(
            PreviewScrollViewer.HorizontalOffset + movedAnchor.X - anchor.X);
        PreviewScrollViewer.ScrollToVerticalOffset(
            PreviewScrollViewer.VerticalOffset + movedAnchor.Y - anchor.Y);
    }

    private void ApplyPreviewScale()
    {
        PreviewCanvas.LayoutTransform = new ScaleTransform(_previewScale, _previewScale);
        ZoomText.Text = $"{Math.Round(_previewScale * 100):0}%";
        ZoomOutButton.IsEnabled = _previewScale > MinPreviewScale + 0.001;
        ZoomInButton.IsEnabled = _previewScale < MaxPreviewScale - 0.001;
    }

    private void ScheduleFitPreviewToViewport()
    {
        if (_previewScaleUserSet || _fitPreviewScheduled)
        {
            return;
        }

        _fitPreviewScheduled = true;
        Dispatcher.BeginInvoke(new Action(() =>
        {
            _fitPreviewScheduled = false;
            if (!_previewScaleUserSet)
            {
                FitPreviewToViewport();
            }
        }), DispatcherPriority.Loaded);
    }

    private void FitPreviewToViewport()
    {
        var pageSize = LayoutEngine.GetPageSize(CurrentOptions);
        var availableWidth = PreviewScrollViewer.ViewportWidth;
        var availableHeight = PreviewScrollViewer.ViewportHeight;
        if (pageSize.Width <= 0 || pageSize.Height <= 0 || availableWidth <= 0 || availableHeight <= 0)
        {
            return;
        }

        var fitScale = Math.Min(availableWidth / pageSize.Width, availableHeight / pageSize.Height) * 0.96;
        _previewScale = Math.Clamp(fitScale, MinPreviewScale, MaxPreviewScale);
        ApplyPreviewScale();
        PreviewScrollViewer.UpdateLayout();
        PreviewScrollViewer.ScrollToHorizontalOffset(0);
        PreviewScrollViewer.ScrollToVerticalOffset(0);
    }

    private void RefreshPreview()
    {
        if (!_viewReady || _isRefreshing)
        {
            return;
        }

        _isRefreshing = true;
        try
        {
            var options = CurrentOptions;
            var pageSize = LayoutEngine.GetPageSize(options);
            PreviewCanvas.Width = pageSize.Width;
            PreviewCanvas.Height = pageSize.Height;
            PreviewCanvas.Children.Clear();

            var placements = PageCount == 0
                ? Array.Empty<LayoutPlacement>()
                : LayoutEngine.Calculate(Items, options, _previewPage, pageSize);

            foreach (var placement in placements)
            {
                AddPreviewItem(placement);
            }
            if (CropMarksCheckBox.IsChecked == true)
            {
                foreach (var line in CreateCropLines(options, placements))
                {
                    PreviewCanvas.Children.Add(line);
                }
            }

            PageText.Text = PageCount == 0
                ? "0 / 0"
                : $"{_previewPage + 1} / {PageCount}";
            SummaryText.Text = Items.Count == 0
                ? "准备好您的第一张 A4"
                : $"{Items.Count} 个内容项 · 预计 {PageCount} 张 A4";
            FileCountText.Text = $"{Items.Count} 项";
            EmptyFilesState.Visibility = Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            PreviewEmptyState.Visibility = Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            PrintButton.IsEnabled = Items.Count > 0 && !ImportBusy;
            ClearButton.IsEnabled = Items.Count > 0 && !ImportBusy;
            PreviousPageButton.IsEnabled = _previewPage > 0;
            NextPageButton.IsEnabled = _previewPage + 1 < PageCount;
            PaperSizeText.Text = options.Orientation == ModelPageOrientation.Landscape
                ? "A4 · 297 × 210 mm"
                : "A4 · 210 × 297 mm";
            LayoutHintText.Text = _itemsPerSheet switch
            {
                1 => "全页排版 · 保持原始比例",
                2 when options.Orientation == ModelPageOrientation.Landscape => "左右排列 · 每张 2 项",
                2 => "上下排列 · 每张 2 项",
                4 => "四宫格 · 每张 4 项",
                _ => string.Empty
            };
            if (!_previewScaleUserSet)
            {
                ScheduleFitPreviewToViewport();
            }
        }
        finally
        {
            _isRefreshing = false;
        }
    }

    private void AddPreviewItem(LayoutPlacement placement)
    {
        var image = new Image
        {
            Source = placement.Image,
            Width = placement.ContentRect.Width,
            Height = placement.ContentRect.Height,
            Stretch = Stretch.Fill,
            SnapsToDevicePixels = true,
            ToolTip = placement.Item.Description
        };
        Canvas.SetLeft(image, placement.ContentRect.Left);
        Canvas.SetTop(image, placement.ContentRect.Top);
        PreviewCanvas.Children.Add(image);

    }

    private static IReadOnlyList<Line> CreateCropLines(
        LayoutOptions options,
        IReadOnlyList<LayoutPlacement> placements)
    {
        var lines = new List<Line>();
        if (placements.Count < 2 || options.ItemsPerSheet == 1)
        {
            return lines;
        }

        if (options.ItemsPerSheet == 2)
        {
            var first = placements[0].CellRect;
            var second = placements[1].CellRect;
            if (Math.Abs(first.Top - second.Top) > 0.1)
            {
                var y = (first.Bottom + second.Top) / 2;
                lines.Add(CreateDashedLine(Math.Min(first.Left, second.Left), y, Math.Max(first.Right, second.Right), y));
            }
            else
            {
                var x = (first.Right + second.Left) / 2;
                lines.Add(CreateDashedLine(x, Math.Min(first.Top, second.Top), x, Math.Max(first.Bottom, second.Bottom)));
            }
            return lines;
        }

        if (placements.Count < 4)
        {
            return lines;
        }

        var ordered = placements
            .OrderBy(p => p.CellRect.Top)
            .ThenBy(p => p.CellRect.Left)
            .ToArray();
        var topLeft = ordered[0].CellRect;
        var topRight = ordered[1].CellRect;
        var bottomLeft = ordered[2].CellRect;
        var verticalX = (topLeft.Right + topRight.Left) / 2;
        var horizontalY = (topLeft.Bottom + bottomLeft.Top) / 2;
        lines.Add(CreateDashedLine(verticalX, topLeft.Top, verticalX, bottomLeft.Bottom));
        lines.Add(CreateDashedLine(topLeft.Left, horizontalY, topRight.Right, horizontalY));
        return lines;
    }

    private static Line CreateDashedLine(double x1, double y1, double x2, double y2)
        => new()
        {
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
            Stroke = new SolidColorBrush(Color.FromArgb(190, 85, 85, 85)),
            StrokeThickness = 0.8,
            StrokeDashArray = new DoubleCollection { 4, 3 },
            IsHitTestVisible = false
        };

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        if (ImportBusy)
        {
            return;
        }

        if (Items.Count == 0)
        {
            MessageBox.Show(this, "请先添加 PDF 或图片。", "无法打印", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var options = CurrentOptions;
            var document = BuildPrintDocument(options);
            var dialog = new PrintDialog();
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var ticket = dialog.PrintTicket ?? new PrintTicket();
            ticket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
            ticket.PageOrientation = options.Orientation == ModelPageOrientation.Landscape
                ? System.Printing.PageOrientation.Landscape
                : System.Printing.PageOrientation.Portrait;
            dialog.PrintTicket = ticket;
            dialog.PrintDocument(document.DocumentPaginator, "发票打印小工具");
            StatusText.Text = $"已提交 {PageCount} 张 A4 打印任务";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "打印失败", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusText.Text = "打印失败";
        }
    }

    private FixedDocument BuildPrintDocument(LayoutOptions options)
    {
        var pageSize = LayoutEngine.GetPageSize(options, 793.7008);
        var document = new FixedDocument();
        document.DocumentPaginator.PageSize = pageSize;

        for (var page = 0; page < PageCount; page++)
        {
            var fixedPage = new FixedPage
            {
                Width = pageSize.Width,
                Height = pageSize.Height,
                Background = Brushes.White
            };
            var placements = LayoutEngine.Calculate(Items, options, page, pageSize);
            foreach (var placement in placements)
            {
                var image = new Image
                {
                    Source = placement.Image,
                    Width = placement.ContentRect.Width,
                    Height = placement.ContentRect.Height,
                    Stretch = Stretch.Fill,
                    SnapsToDevicePixels = true
                };
                FixedPage.SetLeft(image, placement.ContentRect.Left);
                FixedPage.SetTop(image, placement.ContentRect.Top);
                fixedPage.Children.Add(image);

            }

            if (CropMarksCheckBox.IsChecked == true)
            {
                foreach (var line in CreateCropLines(options, placements))
                {
                    fixedPage.Children.Add(line);
                }
            }

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);
            document.Pages.Add(pageContent);
        }

        return document;
    }

    private void Window_KeyDown(object sender, WpfKeyEventArgs e)
    {
        if (ImportBusy && (e.Key == Key.Delete ||
            (Keyboard.Modifiers == ModifierKeys.Control && e.Key is Key.O or Key.P)))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
        {
            AddFiles_Click(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Print_Click(sender, e);
            e.Handled = true;
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key is Key.D0 or Key.NumPad0)
        {
            FitPreview_Click(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.Delete && ItemsList.SelectedItem is ContentItem item)
        {
            Items.Remove(item);
            StatusText.Text = "已删除 1 个内容项";
            e.Handled = true;
        }
    }

    private void SetBusy(bool busy, string? status = null)
    {
        ImportBusy = busy;
        ItemsList.IsEnabled = !busy;
        if (FindName("AddFilesButton") is Control addFilesButton)
        {
            addFilesButton.IsEnabled = !busy;
        }
        ClearButton.IsEnabled = !busy && Items.Count > 0;
        PrintButton.IsEnabled = !busy && Items.Count > 0;
        BusyProgress.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        if (busy)
        {
            SetDropHighlight(false);
        }
        if (status is not null)
        {
            StatusText.Text = status;
        }
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (ImportBusy)
        {
            e.Cancel = true;
            _closeAfterImport = true;
            StatusText.Text = "正在完成文件解析，随后关闭…";
            return;
        }

        _importer.Dispose();
    }
}





