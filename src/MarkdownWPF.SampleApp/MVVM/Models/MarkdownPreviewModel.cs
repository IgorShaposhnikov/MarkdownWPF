using System.Collections.ObjectModel;

namespace MarkdownWPF.SampleApp.Mvvm.Models
{
    public class MarkdownPreviewModel : ObservableObject
    {
        #region Properties


        public ObservableCollection<object> MarkdownElements { get; } = [];


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


        public MarkdownPreviewModel()
        {

        }

        /// <summary>
        /// Parse MarkdownInput property to MarkdownWPF objects and update MarkdownElements collection.
        /// Runs whenever MarkdownInput property changed;
        /// </summary>
        private void OnParseTypedMarkdown()
        {

        }
    }
}
