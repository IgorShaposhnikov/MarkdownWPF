using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class ParagraphRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ParagraphBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ParagraphBlock obj)
        {
            if (renderer.CurrentContext is TextBlock existingTb)
            {
                renderer.WriteLeafInline(obj);
                return;
            }

            var tb = new TextBlock 
            { 
                TextWrapping = TextWrapping.Wrap, 
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 14,
                LineHeight = double.NaN
            };

            if (renderer.CurrentContext == null)
                renderer.RootElements.Add(tb);
            else if (renderer.CurrentContext is StackPanel sp)
                sp.Children.Add(tb);

            renderer.Push(tb);
            renderer.WriteLeafInline(obj);
            renderer.Pop();
        }
    }
}