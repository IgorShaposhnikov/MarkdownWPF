using System.Windows;

namespace MarkdownWPF.Extensions
{
    public static class MarkdownProperties
    {
        public static readonly DependencyProperty IsLastChildProperty =
            DependencyProperty.RegisterAttached(
                "IsLastChild",
                typeof(bool),
                typeof(MarkdownProperties),
                new PropertyMetadata(false));

        public static void SetIsLastChild(UIElement element, bool value)
        {
            element.SetValue(IsLastChildProperty, value);
        }

        public static bool GetIsLastChild(UIElement element)
        {
            return (bool)element.GetValue(IsLastChildProperty);
        }
    }
}
