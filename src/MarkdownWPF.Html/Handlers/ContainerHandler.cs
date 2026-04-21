using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html.Handlers
{
	public class ContainerHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "div", "section", "article", "blockquote", "header", "footer" };

		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var sp = new StackPanel();
			context.RenderNodesToPanel(node.ChildNodes, sp, renderer);
			return sp;
		}
	}
}
