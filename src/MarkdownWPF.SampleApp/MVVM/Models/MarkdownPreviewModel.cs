using Markdig;
using MarkdownWPF.Html;

namespace MarkdownWPF.SampleApp.Mvvm.Models
{
	public sealed class MarkdownPreviewModel : ObservableObject
	{
		private string _markdownInput = string.Empty;
		public string MarkdownInput
		{
			get => _markdownInput; set
			{
				_markdownInput = value;
				OnPropertyChanged();
			}
		}

		public MarkdownPipeline HtmlPipeline { get; }

		public MarkdownPreviewModel()
		{
			HtmlPipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.UseWpfHtml()
				.Build();
		}
	}
}
