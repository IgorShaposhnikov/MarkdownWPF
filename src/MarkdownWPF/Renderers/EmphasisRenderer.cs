using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class EmphasisRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, EmphasisInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, EmphasisInline obj)
        {
            Span span = new Span();

            if (obj.DelimiterChar == '*' || obj.DelimiterChar == '_')
            {
                if (obj.DelimiterCount >= 2) renderer.ApplyStyle(span, MarkdownStyles.StrongEmphasis);
                if (obj.DelimiterCount == 1 || obj.DelimiterCount == 3) renderer.ApplyStyle(span, MarkdownStyles.Emphasis);
            }
            else if (obj.DelimiterChar == '~' && obj.DelimiterCount == 2)
            {
                renderer.ApplyStyle(span, MarkdownStyles.Strikethrough);
            }
            else if (obj.DelimiterChar == '=')
            {
                renderer.ApplyStyle(span, MarkdownStyles.Mark);
            }

            if (renderer.CurrentContext is Span parentSpan) parentSpan.Inlines.Add(span);
            else if (renderer.CurrentContext is TextBlock parentTb) parentTb.Inlines.Add(span);

            renderer.Push(span);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }
}