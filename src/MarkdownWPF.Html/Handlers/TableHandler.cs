using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html.Handlers
{
	public class TableHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "table" };

		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var rows = CollectRows(node);
			if (rows.Count == 0)
				return null;

			var maxCols = rows.Max(r => r.ChildNodes.Count(n => n.NodeType == HtmlNodeType.Element && (n.Name == "td" || n.Name == "th")));

			var grid = new Grid();
			renderer.ApplyStyle(grid, MarkdownStyles.Table);

			for (var i = 0; i < maxCols; i++)
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			for (var rowIdx = 0; rowIdx < rows.Count; rowIdx++)
			{
				var tr = rows[rowIdx];
				grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

				var cells = tr.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && (n.Name == "td" || n.Name == "th")).ToList();

				for (var colIdx = 0; colIdx < cells.Count; colIdx++)
				{
					var cell = cells[colIdx];
					var isHeader = cell.Name == "th";

					var border = new Border();
					renderer.ApplyStyle(border, isHeader ? MarkdownStyles.TableHeaderCell : MarkdownStyles.TableCell);

					Grid.SetRow(border, rowIdx);
					Grid.SetColumn(border, colIdx);
					grid.Children.Add(border);

					var tb = new TextBlock
					{
						VerticalAlignment = VerticalAlignment.Center,
						TextWrapping = TextWrapping.Wrap
					};
					border.Child = tb;

					context.PopulateInlines(cell, tb.Inlines, renderer);
				}
			}

			return grid;
		}

		private static List<HtmlNode> CollectRows(HtmlNode tableNode)
		{
			var rows = new List<HtmlNode>();

			rows.AddRange(tableNode.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && n.Name == "tr"));

			foreach (var section in tableNode.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && (n.Name == "thead" || n.Name == "tbody" || n.Name == "tfoot")))
				rows.AddRange(section.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && n.Name == "tr"));

			return rows;
		}
	}
}
