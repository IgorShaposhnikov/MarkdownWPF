using MarkdownWPF.Models.Inlines;
using MarkdownWPF.Models.Regions;
using MarkdownWPF.Renderer;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace MarkdownWPF.Extensions
{
    public static class TextBlockExtensions
    {
        #region Dependecy Properties


        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(object), typeof(TextBlockExtensions),
                new FrameworkPropertyMetadata(null, OnSourceChanged));

        public static object GetSource(DependencyObject obj)
        {
            return obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, object value)
        {
            obj.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty MarkdownRendererProperty =
            DependencyProperty.RegisterAttached("MarkdownRenderer", typeof(IMarkdownRenderer), typeof(TextBlockExtensions),
                new FrameworkPropertyMetadata(null, OnSourceChanged));

        public static IMarkdownRenderer GetMarkdownRenderer(DependencyObject obj)
        {
            return (IMarkdownRenderer)obj.GetValue(MarkdownRendererProperty);
        }

        public static void SetMarkdownRenderer(DependencyObject obj, IMarkdownRenderer value)
        {
            obj.SetValue(MarkdownRendererProperty, value);
        }


        #endregion Dependency Properties


        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: Везде где можно обойтись без Inlines использовать TextBox для возможности копирования текста
            // Например в блоках кода (не inline)
            if (d is not TextBlock textBlock) return;

            textBlock.Inlines.Clear();

            var source = GetSource(textBlock);

            if (source == null)
                return;

            // Get set value of IMarkdownRenderer
            var render = GetMarkdownRenderer(textBlock);

            if (render == null)
                return;

            if (source is string text)
            {
                textBlock.Inlines.Add(render.RenderInline(new Models.Inlines.Paragraph(text)));
            }
            else if (source is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    // Check the item is a IInline
                    if (item is InlineContainer container)
                    {
                        textBlock.Inlines.AddRange(
                            render.RenderInlineContainer(container)
                            );
                    }
                    else if (item is InlineImage img)
                    {
                        ImageRegion imageRegion = string.IsNullOrEmpty(img.AdditionalUrl) ? new ImageRegion(img) : new ClickableImageRegion(img);
                        var inlineUIContainer = new System.Windows.Documents.InlineUIContainer(GetImage(imageRegion));
                        textBlock.Inlines.Add(inlineUIContainer);
                    }
                    else if (item is IInline mdElement)
                    {
                        // Get renderable inline WPF element (like Run)
                        var inline = render.RenderInline(mdElement);
                        if (inline != null)
                        {
                            textBlock.Inlines.Add(inline);
                        }
                    }
                }
            }
        }

        private static Image GetImage(ImageRegion dataContext)
        {
            var image = new Image();

            // Set MaxHeight with a Binding
            Binding maxHeightBinding = new("Height");
            image.SetBinding(FrameworkElement.MaxHeightProperty, maxHeightBinding);

            // Set Source with a Binding and Converter
            // The converter needs to be retrieved from your application resources
            IValueConverter stringToImageConverter = (IValueConverter)Application.Current.FindResource("StringToImageConverter");
            Binding sourceBinding = new()
            {
                Converter = stringToImageConverter
            };
            image.SetBinding(Image.SourceProperty, sourceBinding);

            // Set other properties that do not require bindings
            image.DataContext = dataContext;
            image.CacheMode = new BitmapCache();
            image.Stretch = Stretch.Uniform;
            image.StretchDirection = StretchDirection.DownOnly;

            if (dataContext is ClickableImageRegion clickableImage)
            {
                image.MouseLeftButtonDown += (s, e) =>
                {
                    OpenUrl(clickableImage.AddtionalUrl);
                };
            }

            return image;
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}