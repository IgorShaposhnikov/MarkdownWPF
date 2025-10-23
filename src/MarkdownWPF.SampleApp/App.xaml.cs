using System.Windows;

namespace MarkdownWPF.SampleApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Add default template

            MarkdownWPFModule.RegisterBlockTemplates(
                Current.Resources
                );

            MarkdownWPFModule.RegisterCustomTemplates(
                Current.Resources, "pack://application:,,,/MarkdownWPF.SampleApp;component/MVVM/Views/MarkdownElementDataTemplates.xaml"
            );
        }
    }
}
