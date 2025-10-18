using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MarkdownWPF.Converters
{
    public sealed class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapImage)
            {
                return value;
            }

            if (value == null)
            {
                return new BitmapImage();
            }

            if (value is string url)
            {
                if (string.IsNullOrEmpty(url))
                {
                    return new BitmapImage();
                }

                // if url is directory path
                if (Directory.Exists(url)) 
                {
                    return new BitmapImage();
                }

                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                if (bitmapImage.IsDownloading)
                {
                    bitmapImage.DownloadCompleted += (s, e) =>
                    {
                        bitmapImage.Freeze();
                    };
                }

                return bitmapImage;
            }

            return new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
