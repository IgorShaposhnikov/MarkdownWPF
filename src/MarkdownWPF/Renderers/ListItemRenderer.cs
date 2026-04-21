using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
	public class ListItemRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ListItemBlock>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, ListItemBlock listItemBlock)
		{
			var grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			renderer.ApplyStyle(grid, MarkdownStyles.ListItem);

			var markerText = "• ";
			if (renderer.CurrentContext is StackPanel parentList && parentList.Tag is int orderNumber && orderNumber != -1)
			{
				var delimiter = ((ListBlock)listItemBlock.Parent).OrderedDelimiter;
				markerText = $"{orderNumber}{delimiter} ";
			}

			var marker = new TextBlock { Text = markerText };
			renderer.ApplyStyle(marker, MarkdownStyles.ListItemMarker);

			Grid.SetColumn(marker, 0);
			grid.Children.Add(marker);

			var contentPanel = new StackPanel();
			Grid.SetColumn(contentPanel, 1);
			grid.Children.Add(contentPanel);

			renderer.Push(contentPanel);
			renderer.WriteChildren(listItemBlock);
			renderer.Pop();

			if (renderer.CurrentContext is Panel parentPanel)
			{
				parentPanel.Children.Add(grid);
			}
		}
	}
}
