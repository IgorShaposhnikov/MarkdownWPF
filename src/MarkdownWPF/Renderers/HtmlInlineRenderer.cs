using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MarkdownWPF.Renderers
{
    public class HtmlInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, HtmlInline>
    {
        private const int BaseFontSize = 12;

        protected override void Write(WpfVirtualizingRenderer renderer, HtmlInline obj)
        {
            double baseFontSize = BaseFontSize;
            if (renderer.ContextElement != null)
            {
                baseFontSize = (double)renderer.ContextElement.GetValue(TextElement.FontSizeProperty);
            }

            var run = new Run(obj.Tag)
            {
                Foreground = Brushes.Gray,
                FontSize = baseFontSize * 0.9
            };

            if (renderer.CurrentContext is Span span) span.Inlines.Add(run);
            else if (renderer.CurrentContext is TextBlock tb) tb.Inlines.Add(run);
        }
    }
}
