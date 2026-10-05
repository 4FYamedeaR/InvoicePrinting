using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using InvoicePrinting;
using InvoicePrinting.Models;
using ModelOrientation = InvoicePrinting.Models.PageOrientation;

namespace Smoke;

/// <summary>
/// Renders the real WPF content tree without displaying a native window. All
/// invoice imagery is synthetic and is created in memory by DrawingVisual.
/// </summary>
internal static class VisualReview
{
    private static readonly List<string> Report = new();
    private static readonly List<string> Failures = new();
    private static string _output = "";

    internal static void Run(bool reducedMotion = false)
    {
        _output = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "artifacts", "ui-review"));
        Directory.CreateDirectory(_output);
        var originalMotion = SystemParameters.ClientAreaAnimation;
        var motionCacheField = typeof(SystemParameters).GetFields(BindingFlags.Static | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.FieldType == typeof(bool) && x.Name.Contains("clientAreaAnimation", StringComparison.OrdinalIgnoreCase));
        if (reducedMotion)
        {
            if (motionCacheField is null) throw new InvalidOperationException("WPF motion preference cache unavailable; cannot exercise reduced-motion startup.");
            motionCacheField.SetValue(null, false);
        }
        var app = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        app.InitializeComponent();
        Report.Add("WPF visual review — actual MainWindow.Content, RenderTargetBitmap at 96 DPI, no Window.Show, no user files.");
        Report.Add($"Host ClientAreaAnimation preference: {SystemParameters.ClientAreaAnimation}.");
        try
        {
            if (reducedMotion) CheckReducedMotionStartup();
            else
            {
                Scenario("01-empty-1440x900", 1440, 900, 0, 1, ModelOrientation.Auto, CheckEmpty);
                Scenario("07-empty-client-820x640", 820, 640, 0, 1, ModelOrientation.Auto, CheckEmpty);
                Scenario("02-five-items-1100x800", 1100, 800, 5, 2, ModelOrientation.Auto, CheckFiveItems);
                Scenario("03-compact-820x680", 820, 680, 5, 2, ModelOrientation.Auto, CheckFit);
                Scenario("06-compact-client-820x640", 820, 640, 5, 2, ModelOrientation.Auto, CheckFit);
                Scenario("04-landscape-four-1600x1000", 1600, 1000, 5, 4, ModelOrientation.Landscape, CheckLandscape);
                CheckBusyAndClear();
            }
        }
        finally
        {
            File.WriteAllLines(Path.Combine(_output, reducedMotion ? "reduced-motion-verification.txt" : "verification.txt"), Report.Concat(Failures.Select(x => "FAIL: " + x)), Encoding.UTF8);
            app.Shutdown();
            if (reducedMotion) motionCacheField!.SetValue(null, originalMotion);
        }

        foreach (var line in Report.Where(x => x.StartsWith("Screenshot:"))) Console.WriteLine(line);
        Console.WriteLine($"Assertions passed: {Report.Count(x => x.StartsWith("PASS:"))}; failures: {Failures.Count}.");
        if (Failures.Count > 0)
            throw new InvalidOperationException(string.Join(Environment.NewLine, Failures));
        Console.WriteLine($"Visual review passed. Screenshots and assertions: {_output}");
    }

    private static void Scenario(string name, int width, int height, int count, int perSheet,
        ModelOrientation orientation, Action<MainWindow, FrameworkElement> assertions)
    {
        var window = CreateWindow();
        try
        {
            var root = (FrameworkElement)window.Content;
            root.Width = width;
            root.Height = height;
            for (var i = 0; i < count; i++) window.Items.Add(CreateInvoice(i));
            SetLayout(window, perSheet, orientation);
            if (count > 0)
                Find<TextBlock>(window, "StatusText").Text = $"已添加 {count} 个内容项 · 可以继续添加或开始打印";
            Layout(root, width, height);
            Invoke(window, "FitPreviewToViewport");
            Layout(root, width, height);
            Save(root, width, height, name);
            Report.Add($"\n{name}: {width}×{height} content area, {count} synthetic invoices, {perSheet} per sheet, {orientation}.");
            CheckBounds(window, root);
            CheckFit(window, root);
            assertions(window, root);
            if (count > 0) CheckPrintParity(window);
        }
        finally { window.Close(); }
    }

    private static MainWindow CreateWindow()
    {
        var window = new MainWindow
        {
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.Manual,
            Left = -20000,
            Top = -20000
        };
        // The real XAML root has an outer Margin. A client-area host preserves
        // that Margin and supplies the Window background without native chrome.
        var content = (UIElement)window.Content;
        window.Content = null;
        window.Content = new Border { Background = window.Background, Child = content };
        return window;
    }

    private static void Layout(FrameworkElement root, double width, double height)
    {
        for (var pass = 0; pass < 4; pass++)
        {
            root.Measure(new Size(width, height));
            root.Arrange(new Rect(0, 0, width, height));
            root.UpdateLayout();
            Drain();
        }
    }

    private static void Drain()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
            new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    private static void Save(FrameworkElement root, int width, int height, string name)
    {
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(root);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        var path = Path.Combine(_output, name + ".png");
        using var stream = File.Create(path);
        encoder.Save(stream);
        Report.Add("Screenshot: " + path);
    }

    private static void SetLayout(MainWindow window, int perSheet, ModelOrientation orientation)
    {
        var root = (FrameworkElement)window.Content;
        var option = Descendants(root).OfType<RadioButton>().FirstOrDefault(x => x.Tag?.ToString() == perSheet.ToString());
        Require(option is not null, $"layout radio for {perSheet} exists");
        if (option is not null)
        {
            option.IsChecked = true;
            option.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }
        var combo = Find<ComboBox>(window, "OrientationCombo");
        combo.SelectedItem = combo.Items.OfType<ComboBoxItem>().First(x => x.Tag?.ToString() == orientation.ToString());
    }

    private static void CheckEmpty(MainWindow window, FrameworkElement root)
    {
        Require(!Find<Button>(window, "PrintButton").IsEnabled, "empty: print disabled");
        Require(!Find<Button>(window, "ClearButton").IsEnabled, "empty: clear disabled");
        Require(!Find<Button>(window, "PreviousPageButton").IsEnabled, "empty: previous page disabled");
        Require(!Find<Button>(window, "NextPageButton").IsEnabled, "empty: next page disabled");
        Require(Find<FrameworkElement>(window, "EmptyFilesState").Visibility == Visibility.Visible, "empty: file invitation visible");
        Require(Find<FrameworkElement>(window, "PreviewEmptyState").Visibility == Visibility.Visible, "empty: preview invitation visible");
        Require(Find<TextBlock>(window, "PageText").Text.Contains("0"), "empty: zero page count visible");
        var invitation = Find<FrameworkElement>(window, "EmptyFilesState");
        var parent = VisualTreeHelper.GetParent(invitation) as FrameworkElement;
        if (parent is not null)
        {
            var bounds = invitation.TransformToAncestor(parent).TransformBounds(new Rect(invitation.RenderSize));
            Require(ContainsWithTolerance(new Rect(parent.RenderSize), bounds), "empty: file invitation fits list row without entering layout controls");
        }
    }

    private static void CheckFiveItems(MainWindow window, FrameworkElement root)
    {
        var list = Find<ListBox>(window, "ItemsList");
        CheckListScroll(list, root);
        var previous = Find<Button>(window, "PreviousPageButton");
        var next = Find<Button>(window, "NextPageButton");
        Require(Find<TextBlock>(window, "PageText").Text.Contains("3"), "five items at 2-up: 3 pages");
        Require(!previous.IsEnabled && next.IsEnabled, "first page: previous disabled, next enabled");
        next.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        next.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Require(previous.IsEnabled && !next.IsEnabled, "last page: previous enabled, next disabled");
        Require(Find<TextBlock>(window, "PageText").Text.Contains("3"), "last page label updated");
        list.SelectedItem = window.Items[0];
        Drain();
        Require(!previous.IsEnabled && next.IsEnabled, "select first invoice navigates to its page");
        Require(list.SelectedIndex == 0, "list selection retained");

        var canvas = Find<Canvas>(window, "PreviewCanvas");
        var combo = Find<ComboBox>(window, "OrientationCombo");
        combo.ApplyTemplate();
        var toggle = combo.Template.FindName("Toggle", combo) as ToggleButton;
        Require(toggle is not null, "orientation combo: toggle exists in custom template");
        if (toggle is not null)
        {
            var binding = BindingOperations.GetBinding(toggle, ToggleButton.IsCheckedProperty);
            Require(binding is not null && binding.Mode == BindingMode.TwoWay && binding.Path.Path == "IsDropDownOpen",
                "orientation combo: toggle has two-way IsDropDownOpen binding");
            Invoke(toggle, "OnClick");
            Drain();
            Report.Add($"Orientation toggle OnClick invoked; unloaded offscreen WPF coerces popup open state to {combo.IsDropDownOpen}. Native popup opening is not asserted by this no-Show harness.");
            combo.SelectedIndex = 2;
            Require(canvas.Width > canvas.Height, "orientation combo: selecting landscape updates page geometry");
            Require(Find<TextBlock>(window, "PaperSizeText").Text.Contains("297 × 210"), "orientation combo: landscape paper dimensions update");
            Require(Find<TextBlock>(window, "LayoutHintText").Text.Contains("左右"), "orientation combo: 2-up landscape hint updates");
            Invoke(toggle, "OnClick");
            Drain();
            Require(!combo.IsDropDownOpen && toggle.IsChecked == false, "orientation combo: second toggle click closes dropdown");
            combo.SelectedIndex = 0;
            Layout(root, root.Width, root.Height);
        }
        var beforeScale = ((ScaleTransform)canvas.LayoutTransform).ScaleX;
        Find<Button>(window, "ZoomInButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Require(((ScaleTransform)canvas.LayoutTransform).ScaleX > beforeScale, "zoom in increases scale");
        Find<Button>(window, "ZoomOutButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Require(Near(((ScaleTransform)canvas.LayoutTransform).ScaleX, beforeScale, 0.002), "zoom out restores prior scale");
        Require(Find<TextBlock>(window, "ZoomText").Text.Contains("%"), "zoom percentage displayed");
        Find<Button>(window, "FitPreviewButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Layout(root, root.Width, root.Height);
        CheckFit(window, root);
        CheckPreviewScroll(window, root);

        var first = window.Items[0];
        var itemButton = Descendants(list).OfType<Button>().FirstOrDefault(x => ReferenceEquals(x.DataContext, first) && x.ToolTip?.ToString()?.Contains("旋转") == true);
        if (itemButton is not null) itemButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        else Invoke(window, "RotateItem_Click", new Button { DataContext = first }, new RoutedEventArgs());
        Require(first.ManualRotation == 90, "rotate action adds 90 degrees");
        var removed = window.Items[^1];
        Invoke(window, "RemoveItem_Click", new Button { DataContext = removed }, new RoutedEventArgs());
        Require(window.Items.Count == 4 && !window.Items.Contains(removed), "remove action removes selected synthetic item");
        Require(Find<TextBlock>(window, "PageText").Text.Contains("2"), "removal updates page count to 2");
        Report.Add("Interactions: actual button routed events for paging/zoom/fit; rotate via realized row when available; remove handler invoked with synthetic row data context.");
    }

    private static void CheckLandscape(MainWindow window, FrameworkElement root)
    {
        var canvas = Find<Canvas>(window, "PreviewCanvas");
        Require(canvas.Width > canvas.Height, "landscape: page wider than tall");
        Require(canvas.Children.OfType<Image>().Count() == 4, "landscape 4-up: exactly 4 invoice images");
        Require(Find<TextBlock>(window, "PageText").Text.Contains("2"), "five items at 4-up: 2 pages");
        var marks = Find<CheckBox>(window, "CropMarksCheckBox");
        marks.IsChecked = true;
        Require(canvas.Children.OfType<System.Windows.Shapes.Line>().Count() == 2, "4-up crop marks: 2 divider lines");
        CheckPrintParity(window);
        marks.IsChecked = false;
    }

    private static void CheckFit(MainWindow window, FrameworkElement root)
    {
        var canvas = Find<Canvas>(window, "PreviewCanvas");
        var viewer = Find<ScrollViewer>(window, "PreviewScrollViewer");
        var scale = (ScaleTransform)canvas.LayoutTransform;
        Require(viewer.ViewportWidth > 0 && viewer.ViewportHeight > 0, "preview viewport has usable dimensions");
        Require(canvas.Width * scale.ScaleX <= viewer.ViewportWidth + 1.5,
            $"fit width {canvas.Width * scale.ScaleX:F1} ≤ viewport {viewer.ViewportWidth:F1}");
        Require(canvas.Height * scale.ScaleY <= viewer.ViewportHeight + 1.5,
            $"fit height {canvas.Height * scale.ScaleY:F1} ≤ viewport {viewer.ViewportHeight:F1}");
    }

    private static void CheckBounds(MainWindow window, FrameworkElement root)
    {
        var extent = new Rect(0, 0, root.ActualWidth, root.ActualHeight);
        var named = new[] { "PrintButton", "ClearButton", "PreviousPageButton", "NextPageButton", "ZoomText", "ZoomOutButton", "ZoomInButton", "PaperSizeText", "LayoutHintText", "FitPreviewButton", "PageText", "SummaryText", "FileCountText", "StatusText" };
        foreach (var name in named)
        {
            var element = Find<FrameworkElement>(window, name);
            if (ActuallyVisible(element) && element.ActualWidth > 0 && element.ActualHeight > 0)
            {
                var rect = element.TransformToAncestor(root).TransformBounds(new Rect(element.RenderSize));
                Require(ContainsWithTolerance(extent, rect), $"bounds: {name} is within root ({rect})");
                if (element is TextBlock text && text.TextWrapping == TextWrapping.NoWrap && text.TextTrimming == TextTrimming.None)
                {
                    var formatted = new FormattedText(text.Text, CultureInfo.GetCultureInfo("zh-CN"), text.FlowDirection,
                        new Typeface(text.FontFamily, text.FontStyle, text.FontWeight, text.FontStretch), text.FontSize,
                        text.Foreground, null, TextOptions.GetTextFormattingMode(text), 1);
                    Require(formatted.WidthIncludingTrailingWhitespace <= text.ActualWidth + 2,
                        $"text: {name} natural {formatted.WidthIncludingTrailingWhitespace:F1} fits allocated {text.ActualWidth:F1} without trimming");
                }
            }
        }
        var buttons = Descendants(root).OfType<ButtonBase>()
            .Where(x => ActuallyVisible(x) && x.ActualWidth > 0 && x.ActualHeight > 0 && !HasAncestor<ScrollViewer>(x));
        foreach (var button in buttons)
        {
            var rect = button.TransformToAncestor(root).TransformBounds(new Rect(button.RenderSize));
            Require(ContainsWithTolerance(extent, rect), $"button within root: {button.Name}/{button.Content}");
        }
    }

    private static void CheckPrintParity(MainWindow window)
    {
        var canvas = Find<Canvas>(window, "PreviewCanvas");
        var options = (LayoutOptions)typeof(MainWindow).GetProperty("CurrentOptions", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        var page = (int)typeof(MainWindow).GetField("_previewPage", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        var document = (FixedDocument)Invoke(window, "BuildPrintDocument", options)!;
        var fixedPage = document.Pages[page].Child;
        var previewImages = canvas.Children.OfType<Image>().ToArray();
        var printImages = fixedPage.Children.OfType<Image>().ToArray();
        Require(printImages.Length == previewImages.Length, "print/preview: same image count");
        for (var i = 0; i < Math.Min(previewImages.Length, printImages.Length); i++)
        {
            var preview = previewImages[i];
            var print = printImages[i];
            Require(ReferenceEquals(preview.Source, print.Source), $"print/preview image {i + 1}: same rotated bitmap");
            Require(Near(Canvas.GetLeft(preview) / canvas.Width, FixedPage.GetLeft(print) / fixedPage.Width), $"print/preview image {i + 1}: normalized left equal");
            Require(Near(Canvas.GetTop(preview) / canvas.Height, FixedPage.GetTop(print) / fixedPage.Height), $"print/preview image {i + 1}: normalized top equal");
            Require(Near(preview.Width / canvas.Width, print.Width / fixedPage.Width), $"print/preview image {i + 1}: normalized width equal");
            Require(Near(preview.Height / canvas.Height, print.Height / fixedPage.Height), $"print/preview image {i + 1}: normalized height equal");
        }
        Require(document.Pages.Count == LayoutEngine.GetPageCount(window.Items.Count, options.ItemsPerSheet), "print: correct FixedDocument page count");
    }

    private static void CheckReducedMotionStartup()
    {
        // The WPF preference cache was overridden before App resource loading.
        // The user's Windows setting is never changed.
        Require(!SystemParameters.ClientAreaAnimation, "reduced motion: in-process WPF preference overridden to false before App initialization");
        var window = CreateWindow();
        try
        {
            var root = (FrameworkElement)window.Content;
            root.Width = 1100;
            root.Height = 800;
            Layout(root, 1100, 800);
            Invoke(window, "Window_Loaded", window, new RoutedEventArgs());
            Require(!Find<FrameworkElement>(window, "WorkspaceGrid").HasAnimatedProperties, "reduced motion: loaded workspace has no opacity animation");
            Invoke(window, "SetBusy", true, "合成验证中");
            var hoverKey = typeof(UIElement).GetField("IsMouseOverPropertyKey", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as DependencyPropertyKey;
            var fit = Find<Button>(window, "FitPreviewButton");
            var writeFlag = typeof(UIElement).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.Name == "WriteFlag" && x.GetParameters().Length == 2);
            if (hoverKey is not null && writeFlag is not null)
            {
                var hoverFlag = Enum.Parse(writeFlag.GetParameters()[0].ParameterType, "IsMouseOverCache");
                writeFlag.Invoke(fit, new[] { hoverFlag, (object)true });
                fit.SetValue(hoverKey, true);
            }
            Layout(root, 1100, 800);
            var progress = Find<ProgressBar>(window, "BusyProgress");
            progress.ApplyTemplate();
            Require(SimulateReadOnlyBoolean(progress, "IsVisible", "IsVisibleCache"),
                "reduced motion: visible progress condition exercised in WPF preference test process");
            Drain();
            var signal = progress.Template.FindName("Signal", progress) as Border;
            Require(signal is not null, "reduced motion: custom progress signal exists");
            Require(signal is not null && !signal.HasAnimatedProperties, "reduced motion: busy signal has no animation");
            Require(progress.IsIndeterminate && progress.Visibility == Visibility.Visible,
                "reduced motion: busy progress remains visible and exposes unknown progress semantics");
            var animated = Descendants(root).OfType<UIElement>().Where(x => x.HasAnimatedProperties).ToArray();
            Require(animated.Length == 0, "reduced motion: visual elements have no animated dependency properties");
            var transformAnimations = Descendants(root).OfType<UIElement>()
                .Count(x => x.RenderTransform.HasAnimatedProperties || x is FrameworkElement fe && fe.LayoutTransform.HasAnimatedProperties);
            Require(transformAnimations == 0, "reduced motion: transforms have no animated properties");
            if (hoverKey is not null && writeFlag is not null) Require(fit.IsMouseOver, "reduced motion: simulated WPF hover exercised");
            else Report.Add("Reduced motion hover simulation unavailable; startup and busy animation assertions were still exercised.");
            Save(root, 1100, 800, "05-reduced-motion-busy-1100x800");
            Invoke(window, "SetBusy", false, null);
            Report.Add("Reduced motion tested at startup by overriding the private WPF preference cache in this isolated process, explicitly invoking loaded handler, and simulating hover via the WPF read-only dependency-property key; no OS/user preference change.");
        }
        finally { window.Close(); }
    }

    private static void CheckBusyAndClear()
    {
        Report.Add("\nBusy/clear behavior with 2 synthetic items.");
        var window = CreateWindow();
        try
        {
            window.Items.Add(CreateInvoice(0));
            window.Items.Add(CreateInvoice(1));
            var first = window.Items[0];
            var root = (FrameworkElement)window.Content;
            root.Width = 1100;
            root.Height = 800;
            Layout(root, 1100, 800);
            Invoke(window, "SetBusy", true, "合成导入测试");
            Require(window.ImportBusy, "busy: ImportBusy state set");
            Require(!Find<ListBox>(window, "ItemsList").IsEnabled, "busy: list editing disabled");
            Require(!Find<Button>(window, "AddFilesButton").IsEnabled, "busy: add disabled");
            Require(!Find<Button>(window, "PrintButton").IsEnabled && !Find<Button>(window, "ClearButton").IsEnabled, "busy: print and clear disabled");
            Require(Find<ProgressBar>(window, "BusyProgress").Visibility == Visibility.Visible, "busy: progress visible");
            Invoke(window, "Clear_Click", Find<Button>(window, "ClearButton"), new RoutedEventArgs());
            Invoke(window, "RotateItem_Click", new Button { DataContext = first }, new RoutedEventArgs());
            Invoke(window, "RemoveItem_Click", new Button { DataContext = first }, new RoutedEventArgs());
            Require(window.Items.Count == 2 && first.ManualRotation == 0, "busy: clear, rotate and remove handlers reject mutation");
            var closeArgs = new System.ComponentModel.CancelEventArgs();
            Invoke(window, "Window_Closing", window, closeArgs);
            Require(closeArgs.Cancel, "busy: close waits for importer instead of disposing in-flight resources");
            Require(Find<TextBlock>(window, "StatusText").Text.Contains("随后关闭"), "busy: pending-close status visible");
            Invoke(window, "SetBusy", false, null);
            Require(!window.ImportBusy && Find<ListBox>(window, "ItemsList").IsEnabled, "ready: editing restored");
            Require(Find<Button>(window, "PrintButton").IsEnabled && Find<Button>(window, "ClearButton").IsEnabled, "ready: print and clear restored");
            Require(Find<ProgressBar>(window, "BusyProgress").Visibility == Visibility.Collapsed, "ready: progress hidden");
            Find<Button>(window, "ClearButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            Require(window.Items.Count == 0, "clear: all synthetic items removed");
            Require(!Find<Button>(window, "PrintButton").IsEnabled && !Find<Button>(window, "ClearButton").IsEnabled, "clear: empty state actions disabled");
            Require(Find<FrameworkElement>(window, "EmptyFilesState").Visibility == Visibility.Visible, "clear: empty file invitation restored");
        }
        finally { window.Close(); }
    }

    private static void CheckListScroll(ListBox list, FrameworkElement root)
    {
        var viewer = Descendants(list).OfType<ScrollViewer>().First();
        Require(viewer.ScrollableHeight > 0, "file list: 5 synthetic rows overflow viewport and enable scrolling");
        viewer.ScrollToVerticalOffset(0);
        Layout(root, root.Width, root.Height);
        viewer.PageDown();
        Layout(root, root.Width, root.Height);
        Require(viewer.VerticalOffset > 0, "file list: PageDown advances scroll offset");
        viewer.ScrollToVerticalOffset(viewer.ScrollableHeight);
        Layout(root, root.Width, root.Height);
        Require(Near(viewer.VerticalOffset, viewer.ScrollableHeight, 1), "file list: scroll-to-end reaches final row");
        CheckScrollTracks(viewer);
        viewer.ScrollToVerticalOffset(0);
        Layout(root, root.Width, root.Height);
    }

    private static void CheckPreviewScroll(MainWindow window, FrameworkElement root)
    {
        var canvas = Find<Canvas>(window, "PreviewCanvas");
        var viewer = Find<ScrollViewer>(window, "PreviewScrollViewer");
        for (var i = 0; i < 30; i++) Find<Button>(window, "ZoomInButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Layout(root, root.Width, root.Height);
        Require(Near(((ScaleTransform)canvas.LayoutTransform).ScaleX, 3), "zoom: maximum clamps at 300%");
        Require(!Find<Button>(window, "ZoomInButton").IsEnabled, "zoom: plus disabled at 300%");
        Require(viewer.ScrollableWidth > 0 && viewer.ScrollableHeight > 0, "preview: high zoom enables horizontal and vertical scrolling");
        viewer.ScrollToHorizontalOffset(viewer.ScrollableWidth);
        viewer.ScrollToVerticalOffset(viewer.ScrollableHeight);
        Layout(root, root.Width, root.Height);
        Require(viewer.HorizontalOffset > 0 && viewer.VerticalOffset > 0, "preview: horizontal and vertical offsets advance");
        CheckScrollTracks(viewer);
        for (var i = 0; i < 30; i++) Find<Button>(window, "ZoomOutButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Layout(root, root.Width, root.Height);
        Require(Near(((ScaleTransform)canvas.LayoutTransform).ScaleX, 0.2), "zoom: minimum clamps at 20%");
        Require(!Find<Button>(window, "ZoomOutButton").IsEnabled, "zoom: minus disabled at 20%");
        Find<Button>(window, "FitPreviewButton").RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Layout(root, root.Width, root.Height);
        Require(Near(viewer.HorizontalOffset, 0) && Near(viewer.VerticalOffset, 0), "fit: scroll offsets reset to zero");
        CheckFit(window, root);
    }

    private static void CheckScrollTracks(ScrollViewer viewer)
    {
        foreach (var bar in Descendants(viewer).OfType<ScrollBar>().Where(x => x.Visibility == Visibility.Visible))
        {
            bar.ApplyTemplate();
            var track = bar.Template.FindName("PART_Track", bar) as Track;
            Require(track is not null && track.Thumb is not null, $"scrollbar {bar.Orientation}: PART_Track and Thumb exist");
            if (track is not null)
            {
                Require(track.DecreaseRepeatButton?.Command is not null && track.IncreaseRepeatButton?.Command is not null,
                    $"scrollbar {bar.Orientation}: both page commands are connected");
                var thickness = bar.Orientation == Orientation.Vertical ? bar.ActualWidth : bar.ActualHeight;
                var thicknessProperty = bar.Orientation == Orientation.Vertical ? FrameworkElement.WidthProperty : FrameworkElement.HeightProperty;
                Require(thickness <= 11,
                    $"scrollbar {bar.Orientation}: thickness {thickness:F1}px ≤11px (value source {DependencyPropertyHelper.GetValueSource(bar, thicknessProperty).BaseValueSource})");
            }
        }
    }

    private static ContentItem CreateInvoice(int index)
    {
        var names = new[] { "电子发票_办公用品.pdf", "增值税发票_差旅费用.pdf", "电子发票_技术服务.pdf", "销售凭证_设备采购.jpg", "电子发票_物业服务.pdf" };
        const int width = 920, height = 560;
        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            var ink = new SolidColorBrush(Color.FromRgb(86, 109, 133));
            var faint = new SolidColorBrush(Color.FromRgb(151, 167, 181));
            var rule = new Pen(faint, 1.2);
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));
            Text(dc, "电子发票（增值税普通发票）", 286, 28, 27, ink);
            Text(dc, "国家税务总局 · 电子发票服务平台", 323, 66, 12, faint);
            dc.DrawLine(new Pen(ink, 2), new Point(270, 91), new Point(676, 91));
            dc.DrawLine(new Pen(ink, 1), new Point(270, 96), new Point(676, 96));
            Text(dc, "发票号码：" + (2468101200000058L + index), 646, 33, 12, ink);
            Text(dc, "开票日期：2026 年 09 月 25 日", 646, 57, 12, ink);
            for (var qy = 0; qy < 15; qy++)
                for (var qx = 0; qx < 15; qx++)
                    if ((qx * 7 + qy * 11 + index) % 4 < 2)
                        dc.DrawRectangle(ink, null, new Rect(35 + qx * 4, 30 + qy * 4, 4, 4));
            dc.DrawRectangle(null, rule, new Rect(32, 116, 856, 390));
            foreach (var y in new[] { 208d, 248, 398, 442, 477 }) dc.DrawLine(rule, new Point(32, y), new Point(888, y));
            dc.DrawLine(rule, new Point(460, 116), new Point(460, 208));
            Text(dc, "购买方信息", 45, 124, 13, ink);
            Text(dc, "销售方信息", 475, 124, 13, ink);
            Text(dc, "名称：上海明序科技有限公司", 45, 155, 15, ink);
            Text(dc, "名称：杭州云际商贸有限公司", 475, 155, 15, ink);
            Text(dc, "统一社会信用代码：91310000MA1A2B3C4D", 45, 183, 11, faint);
            Text(dc, "统一社会信用代码：91330100MA9X8Y7Z6W", 475, 183, 11, faint);
            var columns = new[] { ("项目名称", 45), ("规格型号", 286), ("单位", 407), ("数量", 476), ("单价", 550), ("金额", 656), ("税率", 752), ("税额", 819) };
            foreach (var (label, x) in columns) Text(dc, label, x, 219, 13, ink);
            foreach (var x in new[] { 272d, 393, 463, 536, 639, 738, 805 }) dc.DrawLine(rule, new Point(x, 208), new Point(x, 398));
            Text(dc, "*办公用品* 文具耗材", 45, 269, 14, ink);
            Text(dc, "标准套装", 286, 270, 13, ink);
            Text(dc, "批", 410, 270, 13, ink);
            Text(dc, "1", 490, 270, 13, ink);
            Text(dc, "1,200.00", 550, 270, 13, ink);
            Text(dc, "1,200.00", 649, 270, 13, ink);
            Text(dc, "13%", 753, 270, 13, ink);
            Text(dc, "156.00", 811, 270, 13, ink);
            Text(dc, "合计", 45, 409, 14, ink);
            Text(dc, "¥ 1,200.00", 649, 409, 13, ink);
            Text(dc, "¥ 156.00", 809, 409, 13, ink);
            Text(dc, "价税合计（大写）  壹仟叁佰伍拾陆圆整", 45, 453, 15, ink);
            Text(dc, "（小写） ¥ 1,356.00", 663, 453, 14, ink);
            Text(dc, "备 注    合成票据 · 仅供界面布局验证", 45, 486, 11, faint);
            Text(dc, "开票人：林晓", 44, 522, 12, ink);
            Text(dc, "SYNTHETIC SAMPLE / UI REVIEW", 651, 525, 10, faint);
        }
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(visual);
        bitmap.Freeze();
        return new ContentItem(names[index % names.Length], index == 3 ? ContentKind.Image : ContentKind.PdfPage,
            index == 3 ? null : 1, width, height, bitmap);
    }

    private static void Text(DrawingContext dc, string text, double x, double y, double size, Brush color)
        => dc.DrawText(new FormattedText(text, CultureInfo.GetCultureInfo("zh-CN"), FlowDirection.LeftToRight,
            new Typeface("Microsoft YaHei UI"), size, color, 1), new Point(x, y));

    private static T Find<T>(MainWindow window, string name) where T : class
        => window.FindName(name) as T ?? throw new InvalidOperationException($"Missing {typeof(T).Name}: {name}");

    private static object? Invoke(object target, string name, params object?[] arguments)
    {
        var method = target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Missing method: " + name);
        try { return method.Invoke(target, arguments); }
        catch (TargetInvocationException ex) { throw ex.InnerException ?? ex; }
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject node)
    {
        yield return node;
        if (node is not Visual && node is not System.Windows.Media.Media3D.Visual3D) yield break;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(node); i++)
            foreach (var child in Descendants(VisualTreeHelper.GetChild(node, i))) yield return child;
    }

    private static bool ActuallyVisible(FrameworkElement element)
    {
        DependencyObject? node = element;
        while (node is not null)
        {
            if (node is Window) break;
            if (node is UIElement e && e.Visibility != Visibility.Visible) return false;
            node = VisualTreeHelper.GetParent(node);
        }
        return true;
    }

    private static bool HasAncestor<T>(DependencyObject element) where T : DependencyObject
    {
        var node = VisualTreeHelper.GetParent(element);
        while (node is not null)
        {
            if (node is T) return true;
            node = VisualTreeHelper.GetParent(node);
        }
        return false;
    }

    private static bool SimulateReadOnlyBoolean(UIElement element, string propertyName, string cacheFlag)
    {
        var key = typeof(UIElement).GetField(propertyName + "PropertyKey", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as DependencyPropertyKey;
        var writeFlag = typeof(UIElement).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == "WriteFlag" && x.GetParameters().Length == 2);
        if (key is null || writeFlag is null) return false;
        var flag = Enum.Parse(writeFlag.GetParameters()[0].ParameterType, cacheFlag);
        writeFlag.Invoke(element, new[] { flag, (object)true });
        element.SetValue(key, true);
        return element.GetType().GetProperty(propertyName)?.GetValue(element) is true;
    }

    private static bool ContainsWithTolerance(Rect outer, Rect inner)
        => inner.Left >= outer.Left - 1 && inner.Top >= outer.Top - 1 && inner.Right <= outer.Right + 1 && inner.Bottom <= outer.Bottom + 1;
    private static bool Near(double a, double b, double tolerance = 0.00001) => Math.Abs(a - b) <= tolerance;

    private static void Require(bool condition, string message)
    {
        if (condition) Report.Add("PASS: " + message);
        else Failures.Add(message);
    }
}
