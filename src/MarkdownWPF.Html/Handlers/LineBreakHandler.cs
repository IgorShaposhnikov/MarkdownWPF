using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html.Handlers
{
	public class LineBreakHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "br" };

		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			return new TextBlock { Text = Environment.NewLine };
		}
	}
}
