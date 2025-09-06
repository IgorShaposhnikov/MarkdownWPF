using MarkdownWPF.SampleApp.Mvvm.Models;

namespace MarkdownWPF.SampleApp.Mvvm.ViewModels
{
    public sealed class MarkdownPreviewViewModel : ViewModelBase
    {
        public MarkdownPreviewModel Model { get; }


        public MarkdownPreviewViewModel()
        {
            Model = new MarkdownPreviewModel();
        }
    }
}
