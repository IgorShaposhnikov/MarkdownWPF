using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
	public class BlockQuoteRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, QuoteBlock>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, QuoteBlock quoteBlock)
		{
			var border = new Border();
			renderer.ApplyStyle(border, MarkdownStyles.BlockQuote);
			renderer.HandleLastChildMargin(border, quoteBlock);

			var stackPanel = new StackPanel();
			border.Child = stackPanel;

			if (renderer.CurrentContext == null)
			{
				renderer.RootElements.Add(border);
			}
			else if (renderer.CurrentContext is Panel parentPanel)
			{
				parentPanel.Children.Add(border);
			}

			renderer.Push(stackPanel);
			renderer.WriteChildren(quoteBlock);
			renderer.Pop();
		}
	}
}
