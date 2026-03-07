using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class ListRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ListBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ListBlock listBlock)
        {
            var stackPanel = new StackPanel
            {
                Margin = new Thickness(0, 5, 0, 5)
            };

            int orderStart = 1;
            if (listBlock.IsOrdered && int.TryParse(listBlock.OrderedStart, out int start))
            {
                orderStart = start;
            }

            stackPanel.Tag = listBlock.IsOrdered ? orderStart : -1;

            renderer.Push(stackPanel);

            foreach (var item in listBlock)
            {
                renderer.Write(item);

                if (listBlock.IsOrdered && stackPanel.Tag is int currentOrder)
                {
                    stackPanel.Tag = currentOrder + 1;
                }
            }

            renderer.Pop();

            if (renderer.CurrentContext is Panel parentPanel)
            {
                parentPanel.Children.Add(stackPanel);
            }
            else
            {
                renderer.RootElements.Add(stackPanel);
            }
        }
    }
}
