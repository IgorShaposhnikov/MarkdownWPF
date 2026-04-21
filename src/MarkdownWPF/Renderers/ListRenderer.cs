using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
	public class ListRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ListBlock>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, ListBlock listBlock)
		{
			var stackPanel = new StackPanel();

			renderer.ApplyStyle(stackPanel, MarkdownStyles.List);
			renderer.HandleLastChildMargin(stackPanel, listBlock);

			var orderStart = 1;
			if (listBlock.IsOrdered && int.TryParse(listBlock.OrderedStart, out int start))
			{
				orderStart = start;
			}

			stackPanel.Tag = listBlock.IsOrdered ? orderStart : -1;

			if (renderer.CurrentContext == null)
			{
				renderer.RootElements.Add(stackPanel);
			}
			else if (renderer.CurrentContext is Panel parentPanel)
			{
				parentPanel.Children.Add(stackPanel);
			}

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
		}
	}
}
