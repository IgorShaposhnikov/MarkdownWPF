using Markdig.Extensions.TaskLists;
using Markdig.Renderers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MarkdownWPF.Renderers
{
	public class TaskListRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, TaskList>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, TaskList obj)
		{
			var checkbox = new CheckBox
			{
				IsChecked = obj.Checked,
				IsEnabled = false,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(0, 0, 4, 0)
			};

			if (renderer.CurrentContext is TextBlock tb)
			{
				var container = new InlineUIContainer(checkbox);
				tb.Inlines.Add(container);
			}
		}
	}
}
