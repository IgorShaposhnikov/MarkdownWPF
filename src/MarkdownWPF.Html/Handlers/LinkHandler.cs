using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html.Handlers
{
	public class LinkHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "a" };

		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var tb = new TextBlock 
			{ 
				TextWrapping = TextWrapping.Wrap 
			};

			var link = context.CreateHyperlink(node, renderer);
			context.PopulateInlines(node, link.Inlines, renderer);
			tb.Inlines.Add(link);
			return tb;
		}
	}
}
