using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarkdownWPF.Renderers
{
    public class EmphasisRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, EmphasisInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, EmphasisInline obj)
        {
            Span span = new Span();
            if (obj.DelimiterChar == '*')
            {
                if (obj.DelimiterCount >= 2) span.FontWeight = FontWeights.Bold;
                if (obj.DelimiterCount == 1 || obj.DelimiterCount == 3) span.FontStyle = FontStyles.Italic;
            }
            else if (obj.DelimiterChar == '_')
            {
                if (obj.DelimiterCount >= 2) span.FontWeight = FontWeights.Bold;
                if (obj.DelimiterCount == 1 || obj.DelimiterCount == 3) span.FontStyle = FontStyles.Italic;
            }
            else if (obj.DelimiterChar == '~')
            {
                if (obj.DelimiterCount == 2) span.TextDecorations = TextDecorations.Strikethrough;
                else { span.BaselineAlignment = BaselineAlignment.Subscript; span.FontSize = 11; }
            }
            else if (obj.DelimiterChar == '^') { span.BaselineAlignment = BaselineAlignment.Superscript; span.FontSize = 11; }
            else if (obj.DelimiterChar == '+') span.TextDecorations = TextDecorations.Underline;
            else if (obj.DelimiterChar == '=') span.Background = new SolidColorBrush(Colors.LightYellow);

            if (renderer.CurrentContext is Span parentSpan) parentSpan.Inlines.Add(span);
            else if (renderer.CurrentContext is TextBlock parentTb) parentTb.Inlines.Add(span);

            renderer.Push(span);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }
}