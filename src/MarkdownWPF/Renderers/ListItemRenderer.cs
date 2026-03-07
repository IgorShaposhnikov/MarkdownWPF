using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents; // Нужно для TextElement

namespace MarkdownWPF.Renderers
{
    public class ListItemRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ListItemBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ListItemBlock listItemBlock)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            string markerText = "• ";
            if (renderer.CurrentContext is StackPanel parentList && parentList.Tag is int orderNumber && orderNumber != -1)
            {
                char delimiter = ((ListBlock)listItemBlock.Parent).OrderedDelimiter;
                markerText = $"{orderNumber}{delimiter} ";
            }

            var marker = new TextBlock
            {
                Text = markerText,
                Margin = new Thickness(0, 0, 5, 0)
            };

            var contentPanel = new StackPanel();
            contentPanel.VerticalAlignment = VerticalAlignment.Center;

            stackPanel.Children.Add(marker);
            stackPanel.Children.Add(contentPanel);

            renderer.Push(contentPanel);
            renderer.WriteChildren(listItemBlock);
            renderer.Pop();

            if (renderer.CurrentContext is Panel parentPanel)
            {
                parentPanel.Children.Add(stackPanel);
            }
        }
    }
}
