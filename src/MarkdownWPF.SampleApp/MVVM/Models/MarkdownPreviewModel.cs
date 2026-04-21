namespace MarkdownWPF.SampleApp.Mvvm.Models
{
	public sealed class MarkdownPreviewModel : ObservableObject
	{
		private string _markdownInput;
		public string MarkdownInput
		{
			get => _markdownInput; set
			{
				_markdownInput = value;
				OnPropertyChanged();
			}
		}
	}
}
