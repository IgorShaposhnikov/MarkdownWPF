using MarkdownWPF.Models.Regions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MarkdownWPF.Converters
{
    public sealed class StringToImageConverter : IValueConverter
    {
        private static readonly ConcurrentDictionary<string, BitmapImage> _imageCache = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapImage bitmapImage)
            {
                return bitmapImage;
            }

            if (value == null)
            {
                return GetPlaceholderImage();
            }

            if (value is string url)
            {
                return GetCachedImage(url);
            }

            if (value is ImageRegion rg)
            {
                return GetCachedImage(rg.Value.Url, rg);
            }

            return GetPlaceholderImage();
        }

        private BitmapImage? GetCachedImage(string url, ImageRegion region = null)
        {
            if (string.IsNullOrEmpty(url) || Directory.Exists(url))
            {
                return GetPlaceholderImage();
            }

            if (_imageCache.TryGetValue(url, out var cachedImage))
            {
                UpdateRegionDimensions(region, cachedImage);
                return cachedImage;
            }

            return CreateAndCacheImage(url, region);
        }

        private BitmapImage? CreateAndCacheImage(string url, ImageRegion region)
        {
            var bitmapImage = new BitmapImage();

            try
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.EndInit();

                if (bitmapImage.IsDownloading)
                {
                    var completionLock = new object();
                    var completed = false;

                    bitmapImage.DownloadCompleted += (s, e) =>
                    {
                        lock (completionLock)
                        {
                            bitmapImage.Freeze();
                            UpdateRegionDimensions(region, bitmapImage);

                            // Save to cache loaded image
                            _imageCache.AddOrUpdate(url, bitmapImage, (key, oldValue) => bitmapImage);
                            completed = true;
                            Debug.WriteLine($"Image loaded: {bitmapImage.Width}x{bitmapImage.Height}");
                        }
                    };

                    bitmapImage.DownloadFailed += (s, e) =>
                    {
                        lock (completionLock)
                        {
                            Debug.WriteLine($"Failed to load image: {e.ErrorException.Message}");
                            var placeholder = GetPlaceholderImage();
                            _imageCache.TryAdd(url, placeholder);
                            completed = true;
                        }
                    };

                    // For slow loading timeout
                    Task.Delay(30000).ContinueWith(_ =>
                    {
                        lock (completionLock)
                        {
                            if (!completed)
                            {
                                Debug.WriteLine($"Image load timeout: {url}");
                                var placeholder = GetPlaceholderImage();
                                _imageCache.TryAdd(url, placeholder);
                            }
                        }
                    });
                }
                else
                {
                    bitmapImage.Freeze();
                    UpdateRegionDimensions(region, bitmapImage);
                    _imageCache.TryAdd(url, bitmapImage);
                }
            }
            catch (Exception ex)
            {
                //DebugTools.WriteLine($"Error creating image: {ex.Message}");
                return GetPlaceholderImage();
            }

            return bitmapImage;
        }

        private void UpdateRegionDimensions(ImageRegion region, BitmapImage image)
        {
            if (region != null && image != null)
            {
                region.Width = image.PixelWidth;
                region.Height = image.PixelHeight;
            }
        }

        private BitmapImage? GetPlaceholderImage()
        {
            return null;
        }

        public static void ClearCache()
        {
            _imageCache.Clear();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
