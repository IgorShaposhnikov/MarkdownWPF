using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MarkdownWPF.Renderers
{
	public class CodeBlockRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, CodeBlock>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, CodeBlock obj)
		{
			var text = obj.Lines.ToString();

			var border = new Border();
			renderer.ApplyStyle(border, MarkdownStyles.CodeBlockBorder);
			renderer.HandleLastChildMargin(border, obj);

			var tb = new TextBlock { Text = text };
			renderer.ApplyStyle(tb, MarkdownStyles.CodeBlock);

			var sv = new ScrollViewer
			{
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
				Content = tb
			};

			sv.PreviewMouseWheel += (sender, e) =>
			{
				if (e.Handled)
					return;

				if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
					return;

				e.Handled = true;

				var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
				{
					RoutedEvent = UIElement.MouseWheelEvent
				};

				for (var parent = VisualTreeHelper.GetParent((DependencyObject)sender);
					 parent != null;
					 parent = VisualTreeHelper.GetParent(parent))
				{
					if (parent is ScrollViewer scrollViewer)
					{
						scrollViewer.RaiseEvent(args);
						return;
					}
				}
			};

			border.Child = sv;

			if (renderer.CurrentContext == null)
				renderer.RootElements.Add(border);
			else if (renderer.CurrentContext is StackPanel sp)
				sp.Children.Add(border);
		}
	}
}