namespace MarkdownWPF.SampleApp.Mvvm.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; } = new MarkdownPreviewViewModel();
    }
}
