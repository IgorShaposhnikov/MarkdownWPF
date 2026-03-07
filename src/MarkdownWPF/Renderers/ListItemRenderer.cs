using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class ListItemRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ListItemBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ListItemBlock obj)
        {
            var bulletPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0,0,0,5) };
            var bullet = new TextBlock { Text = "•  ", FontWeight = FontWeights.Bold, FontSize = 16 };
            bulletPanel.Children.Add(bullet);

            var contentPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            bulletPanel.Children.Add(contentPanel);

            if (renderer.CurrentContext is StackPanel parentSp) parentSp.Children.Add(bulletPanel);

            renderer.Push(contentPanel);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }
}