using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MarkdownWPF.Renderers
{
    public class LinkInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, LinkInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, LinkInline obj)
        {
            if (obj.IsImage)
            {
                var image = new Image
                {
                    Stretch = System.Windows.Media.Stretch.Uniform,
                    StretchDirection = StretchDirection.DownOnly,
                    ToolTip = obj.Title,
                    Margin = new Thickness(0, 10, 0, 10),
                    MaxWidth = 600, 
                    MaxHeight = 450,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                if (!string.IsNullOrEmpty(obj.Url))
                {
                    try { image.Source = new BitmapImage(new Uri(obj.Url, UriKind.RelativeOrAbsolute)); }
                    catch { }
                }

                var container = new InlineUIContainer(image) { BaselineAlignment = BaselineAlignment.Bottom };
                if (renderer.CurrentContext is Span s) s.Inlines.Add(container);
                else if (renderer.CurrentContext is TextBlock tb) tb.Inlines.Add(container);
            }
            else
            {
                var hyperlink = new Hyperlink();
                if (!string.IsNullOrEmpty(obj.Url))
                {
                    try
                    {
                        hyperlink.NavigateUri = new Uri(obj.Url, UriKind.RelativeOrAbsolute);
                        hyperlink.Click += (sender, args) =>
                        {
                            var uri = obj.Url;
                            if (uri.StartsWith("#")) return;
                            try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = uri, UseShellExecute = true }); }
                            catch { }
                        };
                    }
                    catch { }
                }

                if (renderer.CurrentContext is Span s) s.Inlines.Add(hyperlink);
                else if (renderer.CurrentContext is TextBlock tb) tb.Inlines.Add(hyperlink);

                renderer.Push(hyperlink);
                renderer.WriteChildren(obj);
                renderer.Pop();
            }
        }
    }
}