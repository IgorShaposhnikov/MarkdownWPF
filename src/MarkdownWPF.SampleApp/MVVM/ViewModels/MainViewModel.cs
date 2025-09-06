namespace MarkdownWPF.SampleApp.Mvvm.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; } = new MarkdownPreviewViewModel();
    }
}
