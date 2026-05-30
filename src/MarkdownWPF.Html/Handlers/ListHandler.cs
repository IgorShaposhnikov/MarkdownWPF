using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html.Handlers
{
	public class ListHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "ul", "ol", "li" };

		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			return node.Name.ToLower() switch
			{
				"ul" => HandleList(node, renderer, context, isOrdered: false, start: 1),
				"ol" => HandleOrderedList(node, renderer, context),
				"li" => HandleListItem(node, renderer, context),
				_ => null
			};
		}

		private static FrameworkElement HandleList(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context, bool isOrdered, int start)
		{
			var stackPanel = new StackPanel();
			stackPanel.Tag = isOrdered ? start : -1;

			renderer.ApplyStyle(stackPanel, MarkdownStyles.List);

			renderer.Push(stackPanel);
			context.RenderNodesToPanel(node.ChildNodes, stackPanel, renderer);
			renderer.Pop();

			return stackPanel;
		}

		private static FrameworkElement HandleOrderedList(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var start = 1;
			var startAttr = node.GetAttributeValue("start", null);
			if (startAttr != null && int.TryParse(startAttr, out var parsedStart))
				start = parsedStart;

			return HandleList(node, renderer, context, isOrdered: true, start);
		}

		private static FrameworkElement HandleListItem(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			renderer.ApplyStyle(grid, MarkdownStyles.ListItem);

			var markerText = "\u2022 ";
			if (renderer.CurrentContext is StackPanel parentList && parentList.Tag is int orderNumber && orderNumber != -1)
			{
				markerText = $"{orderNumber}. ";
				parentList.Tag = orderNumber + 1;
			}

			var marker = new TextBlock { Text = markerText };
			renderer.ApplyStyle(marker, MarkdownStyles.ListItemMarker);

			Grid.SetColumn(marker, 0);
			grid.Children.Add(marker);

			var contentPanel = new StackPanel();
			Grid.SetColumn(contentPanel, 1);
			grid.Children.Add(contentPanel);

			context.RenderNodesToPanel(node.ChildNodes, contentPanel, renderer);

			return grid;
		}
	}
}
