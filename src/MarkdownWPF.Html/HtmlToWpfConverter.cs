using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using MarkdownWPF.Html.Handlers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MarkdownWPF.Html
{
	public class HtmlToWpfConverter
	{
		private readonly Dictionary<string, IHtmlTagHandler> _handlers = new();

		public HtmlToWpfConverter()
		{
			// Регистрируем все наши стратегии
			RegisterHandler(new ContainerHandler());
			RegisterHandler(new HeaderHandler());
			RegisterHandler(new ParagraphHandler());
			RegisterHandler(new ImageHandler());
			RegisterHandler(new LinkHandler());
			RegisterHandler(new LineBreakHandler());
		}

		private void RegisterHandler(IHtmlTagHandler handler)
		{
			foreach(var tag in handler.TargetTags)
				_handlers[tag.ToLower()] = handler;
		}

		// Превращает список узлов HTML в список элементов WPF (для StackPanel, WrapPanel)
		public void RenderNodesToPanel(IEnumerable<HtmlNode> nodes, Panel container, WpfVirtualizingRenderer renderer)
		{
			foreach(var node in nodes)
			{
				var element = ConvertToElement(node, renderer);
				if(element != null) container.Children.Add(element);
			}
		}

		// Превращает один узел в элемент WPF
		public FrameworkElement? ConvertToElement(HtmlNode node, WpfVirtualizingRenderer renderer)
		{
			if(node.NodeType == HtmlNodeType.Text)
			{
				var text = node.InnerText;
				if(string.IsNullOrWhiteSpace(text)) return null;
				var tb = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap };
				renderer.ApplyStyle(tb, MarkdownStyles.Paragraph);
				return tb;
			}

			if(_handlers.TryGetValue(node.Name.ToLower(), out var handler))
				return handler.Handle(node, renderer, this);

			var sp = new StackPanel();
			RenderNodesToPanel(node.ChildNodes, sp, renderer);
			return sp;
		}

		public void PopulateInlines(HtmlNode parentNode, InlineCollection inlines, WpfVirtualizingRenderer renderer)
		{
			foreach(var child in parentNode.ChildNodes)
			{
				if(child.NodeType == HtmlNodeType.Text)
				{
					inlines.Add(new Run(child.InnerText));
				}
				else if(child.NodeType == HtmlNodeType.Element)
				{
					var tag = child.Name.ToLower();
					switch(tag)
					{
						case "br": inlines.Add(new LineBreak()); break;
						case "img":
							var img = CreateWpfImage(child);
							if(img != null) inlines.Add(new InlineUIContainer(img));
							break;
						case "a":
							var link = CreateHyperlink(child, renderer);
							PopulateInlines(child, link.Inlines, renderer); // Рекурсия для вложенных тегов
							inlines.Add(link);
							break;
						case "b" or "strong":
							var b = new Bold(); PopulateInlines(child, b.Inlines, renderer); inlines.Add(b);
							break;
						case "i" or "em":
							var i = new Italic(); PopulateInlines(child, i.Inlines, renderer); inlines.Add(i);
							break;
					}
				}
			}
		}

		// Создание гиперссылки
		public Hyperlink CreateHyperlink(HtmlNode node, WpfVirtualizingRenderer renderer)
		{
			var link = new Hyperlink();
			var href = node.GetAttributeValue("href", "#");
			if(Uri.TryCreate(href, UriKind.RelativeOrAbsolute, out var uri))
			{
				link.NavigateUri = uri;
				link.RequestNavigate += (s, e) => {
					try 
					{
						Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); 
					} 
					catch 
					{ }
					e.Handled = true;
				};
			}

			renderer.ApplyStyle(link, MarkdownStyles.Link);
			return link;
		}

		// Создание картинки (вынеси в отдельный метод, если еще не сделал)
		public Image CreateWpfImage(HtmlNode node)
		{
			var src = node.GetAttributeValue("src", "");
			var img = new Image
			{
				Stretch = Stretch.Uniform,
				StretchDirection = StretchDirection.DownOnly,
				Margin = new Thickness(0, 0, 2, 0),
				VerticalAlignment = VerticalAlignment.Center
			};
			RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
			try
			{
				var bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.UriSource = new Uri(src, UriKind.RelativeOrAbsolute);
				bmp.CacheOption = BitmapCacheOption.OnLoad;
				bmp.EndInit();
				img.Source = bmp;
				if(node.Attributes.Contains("width") && double.TryParse(node.Attributes["width"].Value, out var w)) img.Width = w;
				else if(bmp.PixelWidth > 0) img.MaxWidth = bmp.PixelWidth;
			}
			catch { }
			return img;
		}
	}
}
