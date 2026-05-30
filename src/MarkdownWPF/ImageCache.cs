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
				var bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				if (decodePixelWidth > 0)
					bitmap.DecodePixelWidth = decodePixelWidth;
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
