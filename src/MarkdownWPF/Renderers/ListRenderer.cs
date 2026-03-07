using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class ListRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ListBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ListBlock obj)
        {
            var stackPanel = new StackPanel { Margin = new Thickness(15, 0, 0, 10) };
            if (renderer.CurrentContext == null) renderer.RootElements.Add(stackPanel);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(stackPanel);

            renderer.Push(stackPanel);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }
}