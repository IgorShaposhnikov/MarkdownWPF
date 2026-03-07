using Markdig;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF
{
    public class MarkdownViewer : ItemsControl
    {
        public static readonly DependencyProperty MarkdownProperty =
            DependencyProperty.Register(nameof(Markdown), typeof(string), typeof(MarkdownViewer),
                new PropertyMetadata(string.Empty, OnMarkdownChanged));

        public static readonly DependencyProperty StyleResourceModeProperty =
            DependencyProperty.Register(nameof(StyleResourceMode), typeof(StyleResourceMode), typeof(MarkdownViewer),
            new PropertyMetadata(StyleResourceMode.Static, OnStyleResourceModeChanged));

        public string Markdown
        {
            get => (string)GetValue(MarkdownProperty);
            set => SetValue(MarkdownProperty, value);
        }

        public StyleResourceMode StyleResourceMode
        {
            get => (StyleResourceMode)GetValue(StyleResourceModeProperty);
            set => SetValue(StyleResourceModeProperty, value);
        }

        private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        public MarkdownViewer()
        {
            VirtualizingPanel.SetScrollUnit(this, ScrollUnit.Pixel);
            VirtualizingPanel.SetIsVirtualizing(this, true);
            VirtualizingPanel.SetVirtualizationMode(this, VirtualizationMode.Recycling);
            ScrollViewer.SetCanContentScroll(this, true);

            var factory = new FrameworkElementFactory(typeof(VirtualizingStackPanel));
            ItemsPanel = new ItemsPanelTemplate(factory);

            var template = new ControlTemplate(typeof(ItemsControl));
            var scrollViewer = new FrameworkElementFactory(typeof(ScrollViewer));
            scrollViewer.SetValue(ScrollViewer.PaddingProperty, new Thickness(15));
            scrollViewer.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);

            var presenter = new FrameworkElementFactory(typeof(ItemsPresenter));
            scrollViewer.AppendChild(presenter);
            template.VisualTree = scrollViewer;
            Template = template;
        }

        private static void OnMarkdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MarkdownViewer viewer)
            {
                viewer.RenderMarkdown((string)e.NewValue);
            }
        }

        private static void OnStyleResourceModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MarkdownViewer viewer) 
            {
                viewer.RenderMarkdown(viewer.Markdown);
            }
        }

        private void RenderMarkdown(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ItemsSource = null;
                return;
            }

            var document = Markdig.Markdown.Parse(text, _pipeline);
            var renderer = new WpfVirtualizingRenderer(this, StyleResourceMode);
            var elements = (List<UIElement>)renderer.Render(document);
            ItemsSource = elements;
        }
    }
}