using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class ThematicBreakRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, ThematicBreakBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, ThematicBreakBlock obj)
        {
            var border = new Border();
            renderer.ApplyStyle(border, MarkdownStyles.ThematicBreak);

            if (renderer.CurrentContext == null) renderer.RootElements.Add(border);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(border);
        }
    }
}