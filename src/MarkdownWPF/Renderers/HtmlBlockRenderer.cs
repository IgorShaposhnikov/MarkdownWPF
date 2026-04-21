using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarkdownWPF.Renderers
{
	public class HtmlBlockRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, HtmlBlock>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, HtmlBlock obj)
		{
			var content = string.Join("\n", obj.Lines);

			var tb = new TextBlock
			{
				Text = content,
				Foreground = Brushes.Gray,
				FontStyle = System.Windows.FontStyles.Italic,
				TextWrapping = System.Windows.TextWrapping.Wrap
			};

			renderer.HandleLastChildMargin(tb, obj);

			if (renderer.CurrentContext == null)
				renderer.RootElements.Add(tb);
			else if (renderer.CurrentContext is Panel p)
				p.Children.Add(tb);
		}
	}
}
