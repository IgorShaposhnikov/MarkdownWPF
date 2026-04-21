using Markdig;
using Markdig.Renderers;
using MarkdownWPF.Html.Renderers;

namespace MarkdownWPF.Html
{
	public class WpfHtmlExtension : IMarkdownExtension
	{
		public void Setup(MarkdownPipelineBuilder pipeline) { }
		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			if(renderer is WpfVirtualizingRenderer wpfRenderer)
			{
				wpfRenderer.ReplaceRenderer(new AdvancedHtmlBlockRenderer());
				wpfRenderer.ReplaceRenderer(new AdvancedHtmlInlineRenderer());
			}
		}
	}
}
