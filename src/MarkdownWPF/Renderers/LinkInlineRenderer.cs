using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace MarkdownWPF.Renderers
{
    public class LinkInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, LinkInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, LinkInline obj)
        {
            if (obj.IsImage)
            {
                // --- РЕНДЕРИНГ КАРТИНОК ---
                var inlineContainer = new InlineUIContainer();

                try
                {
                    if (Uri.TryCreate(obj.Url, UriKind.RelativeOrAbsolute, out var uri))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = uri;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Асинхронная загрузка, чтобы не вис UI
                        bitmap.EndInit();

                        var image = new Image
                        {
                            Source = bitmap,
                            Stretch = Stretch.Uniform,
                            Margin = new Thickness(0, 5, 0, 5)
                        };

                        // Можно ограничить максимальную ширину/высоту картинки
                        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
                        inlineContainer.Child = image;
                    }
                }
                catch
                {
                    // Если URL битый, выводим заглушку
                    var tb = new TextBlock { Text = $"[Image: {obj.Url}]", Foreground = Brushes.Red };
                    inlineContainer.Child = tb;
                }

                // Вставляем картинку в текущий контекст (например, в Paragraph или внутрь Hyperlink)
                if (renderer.CurrentContext is Span s) s.Inlines.Add(inlineContainer);
                else if (renderer.CurrentContext is TextBlock t) t.Inlines.Add(inlineContainer);

                return;
            }

            // --- РЕНДЕРИНГ ССЫЛОК ---
            var link = new Hyperlink();
            if (Uri.TryCreate(obj.Url, UriKind.RelativeOrAbsolute, out var linkUri))
            {
                link.NavigateUri = linkUri;

                // Делаем ссылку кликабельной, чтобы она открывалась в браузере
                link.RequestNavigate += (sender, e) =>
                {
                    try { Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); }
                    catch { /* Игнорируем ошибки открытия */ }
                    e.Handled = true;
                };
            }

            renderer.ApplyStyle(link, MarkdownStyles.Link);

            if (renderer.CurrentContext is Span span) span.Inlines.Add(link);
            else if (renderer.CurrentContext is TextBlock tb) tb.Inlines.Add(link);

            // КРИТИЧНО ВАЖНО: пушим ссылку в контекст и рендерим её "детей".
            // Если внутри ссылки есть текст или картинка, они будут отрендерены внутрь этой ссылки.
            renderer.Push(link);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }
}
