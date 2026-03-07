using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarkdownWPF.Renderers
{
    public class ThematicBreakRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ThematicBreakBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ThematicBreakBlock obj)
        {
            var border = new Border
            {
                Height = 2,
                Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                Margin = new Thickness(0, 15, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            if (renderer.CurrentContext == null) renderer.RootElements.Add(border);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(border);
        }
    }
}