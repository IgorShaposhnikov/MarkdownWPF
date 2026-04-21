using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html.Handlers
{
	public class HeaderHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "h1", "h2", "h3", "h4", "h5", "h6" };

		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var level = int.Parse(node.Name.Substring(1));
			var tb = new TextBlock { TextWrapping = TextWrapping.Wrap };

			var styleKey = level switch
			{
				1 => MarkdownStyles.Heading1,
				2 => MarkdownStyles.Heading2,
				3 => MarkdownStyles.Heading3,
				4 => MarkdownStyles.Heading4,
				5 => MarkdownStyles.Heading5,
				_ => MarkdownStyles.Heading6
			};

			renderer.ApplyStyle(tb, styleKey);
			context.PopulateInlines(node, tb.Inlines, renderer);
			return tb;
		}
	}
}
