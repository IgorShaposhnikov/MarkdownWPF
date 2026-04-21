using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html.Handlers
{
	public class ParagraphHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "p" };

		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var align = node.GetAttributeValue("align", "left").ToLower();
			if(node.Descendants("img").Any())
			{
				var wp = new WrapPanel
				{
					HorizontalAlignment = align == "center" ? HorizontalAlignment.Center : HorizontalAlignment.Left,
					Margin = new Thickness(0, 0, 0, 16)
				};
				context.RenderNodesToPanel(node.ChildNodes, wp, renderer);
				return wp;
			}

			var tb = new TextBlock
			{
				TextWrapping = TextWrapping.Wrap,
				TextAlignment = align == "center" ? TextAlignment.Center : TextAlignment.Left,
				Margin = new Thickness(0, 0, 0, 16)
			};
			renderer.ApplyStyle(tb, MarkdownStyles.Paragraph);
			context.PopulateInlines(node, tb.Inlines, renderer);
			return tb;
		}
	}
}
