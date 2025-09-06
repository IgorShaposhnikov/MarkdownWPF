using MarkdownWPF.Models;
using System.Collections.ObjectModel;

namespace MarkdownWPF.SampleApp.Mvvm.Models
{
    public sealed class MarkdownPreviewModel : ObservableObject
    {
        private readonly MarkdownParser _parser = new();


        #region Properties


        public ObservableCollection<IMarkdownElement> MarkdownElements { get; } = [];

        private string _markdownInput;
        public string MarkdownInput
        {
            get => _markdownInput; set
            {
                _markdownInput = value;
                OnPropertyChanged();
                OnParseTypedMarkdown();
            }
        }


        #endregion Properties


        /// <summary>
        /// Parse MarkdownInput property to MarkdownWPF objects and update MarkdownElements collection.
        /// Runs whenever MarkdownInput property changed;
        /// </summary>
        private void OnParseTypedMarkdown()
        {
            var elements = _parser.Parse(MarkdownInput);
            MarkdownElements.Clear();
            foreach (var element in elements) 
            {
                MarkdownElements.Add(element);
            }
        }
    }
}
