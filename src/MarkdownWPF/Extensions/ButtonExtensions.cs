using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Extensions
{
    public static class ButtonExtensions
    {
        public static readonly DependencyProperty UrlBindingProperty = 
            DependencyProperty.RegisterAttached("Url", typeof(string), typeof(ButtonExtensions), 
                new PropertyMetadata(null, OnUrlBindingChanged));

        public static string GetUrl(DependencyObject obj)
        {
            return (string)obj.GetValue(UrlBindingProperty);
        }

        public static void SetUrl(DependencyObject obj, string value)
        {
            obj.SetValue(UrlBindingProperty, value);
        }

        private static void OnUrlBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button _this) 
            {
                _this.Click -= OnButtonClicked;
                var url = GetUrl(d);

                if (string.IsNullOrWhiteSpace(url)) 
                {
                    return;
                }

                _this.Click += OnButtonClicked;
            }
        }

        private static void OnButtonClicked(object sender, RoutedEventArgs e) 
        {
            OpenUrl(GetUrl(sender as DependencyObject));
        }

        private static void OpenUrl(string url) 
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
