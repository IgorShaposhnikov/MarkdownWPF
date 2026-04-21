using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MarkdownWPF.Renderers
{
    public class LinkInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, LinkInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, LinkInline obj)
        {
            if (obj.IsImage)
            {
                RenderImage(renderer, obj);
                return;
            }

            var link = new Hyperlink();
            var url = obj.Url ?? string.Empty;

            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var linkUri))
            {
                link.NavigateUri = linkUri;
                link.RequestNavigate += (sender, e) =>
                {
                    try
                    {
                        // http/https
                        if (e.Uri.Scheme == Uri.UriSchemeHttp || e.Uri.Scheme == Uri.UriSchemeHttps || e.Uri.Scheme == Uri.UriSchemeMailto)
                        {
                            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                        }
                    }
                    catch { }
                    e.Handled = true;
                };
            }

            renderer.ApplyStyle(link, MarkdownStyles.Link);

            if (renderer.CurrentContext is Span span) 
                span.Inlines.Add(link);
            else if (renderer.CurrentContext is TextBlock tb) 
                tb.Inlines.Add(link);

            renderer.Push(link);

            if (obj.FirstChild == null)
                link.Inlines.Add(new Run(url));
            else
                renderer.WriteChildren(obj);

            renderer.Pop();
        }

        private void RenderImage(WpfVirtualizingRenderer renderer, LinkInline obj) 
        {
            var inlineContainer = new InlineUIContainer();

            try
            {
                if (Uri.TryCreate(obj.Url, UriKind.RelativeOrAbsolute, out var uri))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;

                    bitmap.DecodePixelWidth = renderer.OptimalImageDecodeWidth;
                    bitmap.EndInit();

                    var image = new Image
                    {
                        Source = bitmap,
                        Stretch = Stretch.Uniform,
                        Margin = new Thickness(0, 8, 0, 8),
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

                    if (renderer.ContextElement != null)
                    {
                        var binding = new Binding(nameof(FrameworkElement.ActualWidth))
                        {
                            Source = renderer.ContextElement
                        };
                        BindingOperations.SetBinding(image, FrameworkElement.MaxWidthProperty, binding);
                    }

                    inlineContainer.Child = image;
                }
            }
            catch
            {
                var tb = new TextBlock { Text = $"[Image: {obj.Url}]", Foreground = Brushes.Red };
                inlineContainer.Child = tb;
            }

            if (renderer.CurrentContext is Span s) s.Inlines.Add(inlineContainer);
            else if (renderer.CurrentContext is TextBlock t) t.Inlines.Add(inlineContainer);
        }
    }
}
