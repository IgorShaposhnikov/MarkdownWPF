using HtmlAgilityPack;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MarkdownWPF.Html.Renderers
{
	public class AdvancedHtmlInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, Markdig.Syntax.Inlines.HtmlInline>
	{
		private readonly HtmlToWpfConverter _converter = new();

		protected override void Write(WpfVirtualizingRenderer renderer, Markdig.Syntax.Inlines.HtmlInline obj)
		{
			var tagText = obj.Tag.Trim();

			// 1. Обработка ЗАКРЫВАЮЩИХ тегов (</a>, </b>, </i> и т.д.)
			if(tagText.StartsWith("</"))
			{
				// Выходим из контекста только если мы в ссылке или спане
				if(renderer.CurrentContext is Hyperlink || renderer.CurrentContext is Span)
				{
					renderer.Pop();
				}
				return;
			}

			// 2. Для остальных тегов используем парсер
			var doc = new HtmlDocument();
			doc.LoadHtml(tagText);
			var node = doc.DocumentNode.FirstChild;

			if(node == null) return;

			var tagName = node.Name.ToLower();

			// Если это открытие ссылки <a>
			if(tagName == "a")
			{
				var link = _converter.CreateHyperlink(node, renderer);
				AddInlineToContext(renderer, link);
				renderer.Push(link); // Теперь всё последующее будет ВНУТРИ ссылки до появления </a>
			}
			// Если это одиночные теги (картинка или перенос)
			else if(tagName == "img" || tagName == "br")
			{
				var element = _converter.ConvertToElement(node, renderer);
				if(element is Image img)
					AddInlineToContext(renderer, new InlineUIContainer(img));
				else if(tagName == "br")
					AddInlineToContext(renderer, new LineBreak());
			}
			// Если это открытие форматирования (<b>, <i>, <u>, <s>, <code>)
			else if(new[] { "b", "strong", "i", "em", "u", "ins", "s", "strike", "del" }.Contains(tagName))
			{
				var span = tagName switch
				{
					"b" or "strong" => new Bold(),
					"i" or "em" => new Italic(),
					"u" or "ins" => new Underline(),
					"s" or "strike" or "del" => new Span { TextDecorations = TextDecorations.Strikethrough },
					_ => new Span()
				};
				AddInlineToContext(renderer, span);
				renderer.Push(span);
			}
			else if(tagName == "code")
			{
				var codeSpan = new Span { FontFamily = new FontFamily("Consolas") };
				AddInlineToContext(renderer, codeSpan);
				renderer.Push(codeSpan);
			}
		}

		private void AddInlineToContext(WpfVirtualizingRenderer renderer, System.Windows.Documents.Inline inline)
		{
			if(renderer.CurrentContext is TextBlock tb) tb.Inlines.Add(inline);
			else if(renderer.CurrentContext is Span span) span.Inlines.Add(inline);
		}
	}
}
