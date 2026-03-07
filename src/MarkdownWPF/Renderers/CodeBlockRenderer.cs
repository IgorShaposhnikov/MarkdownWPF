using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class CodeBlockRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, CodeBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, CodeBlock obj)
        {
            var text = obj.Lines.ToString();

            var border = new Border();
            renderer.ApplyStyle(border, MarkdownStyles.CodeBlockBorder);

            var tb = new TextBlock { Text = text };
            renderer.ApplyStyle(tb, MarkdownStyles.CodeBlock);

            var sv = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, Content = tb };
            border.Child = sv;

            if (renderer.CurrentContext == null) renderer.RootElements.Add(border);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(border);
        }
    }
}