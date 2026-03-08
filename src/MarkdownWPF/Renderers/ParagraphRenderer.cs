using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class ParagraphRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ParagraphBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ParagraphBlock obj)
        {
            if (renderer.CurrentContext is TextBlock)
            {
                renderer.WriteLeafInline(obj);
                return;
            }

            var tb = new TextBlock();
            renderer.ApplyStyle(tb, MarkdownStyles.Paragraph);
            renderer.HandleLastChildMargin(tb, obj);

            if (renderer.CurrentContext == null) renderer.RootElements.Add(tb);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(tb);

            renderer.Push(tb);
            renderer.WriteLeafInline(obj);
            renderer.Pop();
        }
    }
}