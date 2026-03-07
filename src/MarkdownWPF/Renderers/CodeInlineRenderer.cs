using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarkdownWPF.Renderers
{
    public class CodeInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, CodeInline>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, CodeInline obj)
        {
            var run = new Run(" " + obj.Content + " ")
            {
                FontFamily = new FontFamily("Consolas"),
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Foreground = new SolidColorBrush(Color.FromRgb(199, 37, 78))
            };
            if (renderer.CurrentContext is Span parentSpan) parentSpan.Inlines.Add(run);
            else if (renderer.CurrentContext is TextBlock parentTb) parentTb.Inlines.Add(run);
        }
    }
}