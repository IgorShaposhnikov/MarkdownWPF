using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows.Documents;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class LiteralInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, LiteralInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, LiteralInline obj)
        {
            var run = new Run(obj.Content.ToString());
            if (renderer.CurrentContext is Span span) span.Inlines.Add(run);
            else if (renderer.CurrentContext is TextBlock tb) tb.Inlines.Add(run);
        }
    }
}