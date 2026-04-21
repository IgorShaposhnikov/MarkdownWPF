using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MarkdownWPF.Renderers
{
	public class CodeInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, CodeInline>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, CodeInline obj)
		{
			var run = new Run(obj.Content);
			renderer.ApplyStyle(run, MarkdownStyles.CodeInline);

			if (renderer.CurrentContext is Span span) 
				span.Inlines.Add(run);
			else if (renderer.CurrentContext is TextBlock tb) 
				tb.Inlines.Add(run);
		}
	}
}