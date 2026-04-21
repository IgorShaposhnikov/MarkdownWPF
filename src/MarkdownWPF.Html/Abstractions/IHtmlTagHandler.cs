using HtmlAgilityPack;
using System.Windows;

namespace MarkdownWPF.Html.Abstractions
{
	public interface IHtmlTagHandler
	{
		string[] TargetTags { get; }
		FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context);
	}
}
