using System.Collections.Concurrent;
using System.Windows.Media.Imaging;

namespace MarkdownWPF
{
	public static class ImageCache
	{
		private static readonly ConcurrentDictionary<string, BitmapImage> _cache = new();

		public static BitmapImage GetOrAdd(string url, int decodePixelWidth = 0)
		{
			return _cache.GetOrAdd(url, _ =>
			{
				var uri = new Uri(url, UriKind.RelativeOrAbsolute);
				var targetWidth = 0;

				if(decodePixelWidth > 0)
				{
					try
					{
						var decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
						var frame = decoder.Frames[0];

						// Only downscale if the original image is wider than our optimal width
						if(frame.PixelWidth > decodePixelWidth)
						{
							targetWidth = decodePixelWidth;
						}
					}
					catch
					{
						// Fallback if decoder inspection fails
						targetWidth = decodePixelWidth;
					}
				}

				var bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.UriSource = uri;
				bitmap.CacheOption = BitmapCacheOption.OnLoad;

				// Only caps large images
				if(targetWidth > 0)
					bitmap.DecodePixelWidth = targetWidth; 

				bitmap.EndInit();
				return bitmap;
			});
		}

		public static void Clear()
		{
			_cache.Clear();
		}
	}
}
