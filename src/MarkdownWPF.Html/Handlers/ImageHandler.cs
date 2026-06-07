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
				var bmp = ImageCache.GetOrAdd(src);
				img.Source = bmp;

				if(node.Attributes.Contains("width") && double.TryParse(node.Attributes["width"].Value, out var w)) 
					img.Width = w;
				else if(bmp.PixelWidth > 0) 
					img.MaxWidth = bmp.PixelWidth;

				// Force layout remeasure when async download completes
				if(bmp.IsDownloading)
				{
					bmp.DownloadCompleted += (s, e) =>
					{
						img.Dispatcher.BeginInvoke(new Action(() =>
						{
							if(node.Attributes.Contains("width") && double.TryParse(node.Attributes["width"].Value, out var w2))
								img.Width = w2;
							else if(bmp.PixelWidth > 0)
								img.MaxWidth = bmp.PixelWidth;

							img.InvalidateMeasure();

							DependencyObject parent = img;
							while(parent != null)
							{
								if(parent is FrameworkElement fe)
								{
									fe.InvalidateMeasure();
								}

								// Safe tree-traversal
								parent = (parent is Visual)
									? VisualTreeHelper.GetParent(parent)
									: LogicalTreeHelper.GetParent(parent);
							}
						}));
					};
				}
			}
			catch { }
			return img;
		}
	}
}
