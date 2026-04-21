using HtmlAgilityPack;
using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Html
{
	public class AdvancedHtmlBlockRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, HtmlBlock>
	{
		private static readonly string[] _sourceArray = new[] { "a", "img", "br", "b", "strong", "i", "em", "u", "span" };
		private readonly HtmlToWpfConverter _converter = new();

		protected override void Write(WpfVirtualizingRenderer renderer, HtmlBlock obj)
		{
			var html = obj.Lines.ToString();
			var doc = new HtmlDocument();
			doc.LoadHtml(html);

			var isInlineOnly = doc.DocumentNode.ChildNodes.All(n => n.NodeType == HtmlNodeType.Text || (_sourceArray).Contains(n.Name.ToLower()));

			if(isInlineOnly)
			{
				var tb = new TextBlock
				{
					TextWrapping = TextWrapping.Wrap,
					Margin = new Thickness(0, 0, 0, 16)
				};

				renderer.ApplyStyle(tb, MarkdownStyles.Paragraph);
				_converter.PopulateInlines(doc.DocumentNode, tb.Inlines, renderer);
				AddElement(renderer, tb, obj);
			}
			else
			{
				var sp = new StackPanel();
				_converter.RenderNodesToPanel(doc.DocumentNode.ChildNodes, sp, renderer);
				AddElement(renderer, sp, obj);
			}
		}

		private void AddElement(WpfVirtualizingRenderer renderer, FrameworkElement el, HtmlBlock obj)
		{
			renderer.HandleLastChildMargin(el, obj);
			if(renderer.CurrentContext == null)
				renderer.RootElements.Add(el);
			else if(renderer.CurrentContext is Panel p)
				p.Children.Add(el);
		}
	}
}
