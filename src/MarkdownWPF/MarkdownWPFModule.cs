using System.Windows;

namespace MarkdownWPF
{
    public static class MarkdownWPFModule
    {
        /// <summary>
        /// Add to app resource dictionary implementation of datatemplates
        /// </summary>
        /// <param name="resourceDictionary">Current app resource dictionary</param>
        /// <param name="path">Custom path to resource dictionary with datatemplates</param>
        public static void RegisterBlockTemplates(this ResourceDictionary resourceDictionary, string path = null)
        {
            path = string.IsNullOrEmpty(path)
                ? "pack://application:,,,/MarkdownWPF;component/MarkdownElementDataTemplates.xaml"
                : path;

            resourceDictionary.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(path, UriKind.RelativeOrAbsolute)
            });
        }

        public static void RegisterCustomTemplates(this ResourceDictionary resourceDictionary, string path = null) 
        {
            resourceDictionary.MergedDictionaries.Add(new ResourceDictionary()
            {
                Source = new Uri(path, UriKind.RelativeOrAbsolute)
            });
        }
    }
}
