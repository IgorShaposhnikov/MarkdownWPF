using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
	public class HeadingRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, HeadingBlock>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, HeadingBlock obj)
		{
			var tb = new TextBlock();

			var styleKey = obj.Level switch
			{
				1 => MarkdownStyles.Heading1,
				2 => MarkdownStyles.Heading2,
				3 => MarkdownStyles.Heading3,
				4 => MarkdownStyles.Heading4,
				5 => MarkdownStyles.Heading5,
				_ => MarkdownStyles.Heading6
			};

			renderer.ApplyStyle(tb, styleKey);
			renderer.HandleLastChildMargin(tb, obj);

			if (renderer.CurrentContext == null)
				renderer.RootElements.Add(tb);
			else if (renderer.CurrentContext is StackPanel sp)
				sp.Children.Add(tb);

			renderer.Push(tb);
			renderer.WriteLeafInline(obj);
			renderer.Pop();
		}
	}
}