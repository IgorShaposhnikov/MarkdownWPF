using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows.Documents;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class LineBreakInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, LineBreakInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, LineBreakInline obj)
        {
            string content = obj.IsHard ? "\n" : " ";
            var run = new Run(content);
            if (renderer.CurrentContext is Span span) span.Inlines.Add(run);
            else if (renderer.CurrentContext is TextBlock tb) tb.Inlines.Add(run);
        }
    }
}