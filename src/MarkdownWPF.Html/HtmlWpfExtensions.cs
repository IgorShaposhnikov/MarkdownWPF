using Markdig;

namespace MarkdownWPF.Html
{
	public static class HtmlWpfExtensions
	{
		public static MarkdownPipelineBuilder UseWpfHtml(this MarkdownPipelineBuilder pipeline)
		{
			pipeline.Extensions.Add(new WpfHtmlExtension());
			return pipeline;
		}
	}
}
