using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarkdownWPF.Renderers
{
    public class CodeBlockRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, CodeBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, CodeBlock obj)
        {
            var text = obj.Lines.ToString();
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10)
            };
            var tb = new TextBlock { Text = text, FontFamily = new FontFamily("Consolas"), TextWrapping = TextWrapping.NoWrap, FontSize = 13 };
            var sv = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, Content = tb };
            border.Child = sv;

            if (renderer.CurrentContext == null) renderer.RootElements.Add(border);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(border);
        }
    }
}