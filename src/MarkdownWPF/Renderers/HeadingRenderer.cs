using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class HeadingRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, HeadingBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, HeadingBlock obj)
        {
            var tb = new TextBlock();

            // Format the key to match standard convention, e.g., "MarkdownHeading1Style"
            string styleKey = $"MarkdownHeading{obj.Level}Style";

            // Try to find the user-defined style in the application resources
            var userStyle = Application.Current.TryFindResource(styleKey) as Style;

            if (userStyle != null)
            {
                tb.Style = userStyle;
            }
            else
            {
                // Fallback to default styling if the user didn't define a style for this heading level
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Margin = new Thickness(0, 15, 0, 10);
                tb.FontWeight = FontWeights.Bold;

                tb.FontSize = obj.Level switch
                {
                    1 => 32,
                    2 => 24,
                    3 => 20,
                    4 => 18,
                    5 => 16,
                    _ => 14
                };
            }

            if (renderer.CurrentContext == null) renderer.RootElements.Add(tb);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(tb);

            renderer.Push(tb);
            renderer.WriteLeafInline(obj);
            renderer.Pop();
        }
    }
}