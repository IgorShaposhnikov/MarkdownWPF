using HtmlAgilityPack;
using MarkdownWPF.Html.Abstractions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MarkdownWPF.Html.Handlers
{
	public class ImageHandler : IHtmlTagHandler
	{
		public string[] TargetTags => new[] { "img" };
		public FrameworkElement? Handle(HtmlNode node, WpfVirtualizingRenderer renderer, HtmlToWpfConverter context)
		{
			var src = node.GetAttributeValue("src", "");
			var img = new Image
			{
				Stretch = Stretch.Uniform,
				StretchDirection = StretchDirection.DownOnly,
				Margin = new Thickness(2),
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
