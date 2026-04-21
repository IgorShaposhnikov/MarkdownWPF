using Markdig;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MarkdownWPF
{
	public class MarkdownViewer : ItemsControl
	{
		private static readonly MarkdownPipeline _defaultPipeline = new MarkdownPipelineBuilder()
			.UseAdvancedExtensions()
			.Build();

		public static readonly DependencyProperty PipelineProperty =
			DependencyProperty.Register(nameof(Pipeline), typeof(MarkdownPipeline), typeof(MarkdownViewer),
				new PropertyMetadata(null, OnMarkdownChanged));

		public static readonly DependencyProperty MarkdownProperty =
			DependencyProperty.Register(nameof(Markdown), typeof(string), typeof(MarkdownViewer),
				new PropertyMetadata(string.Empty, OnMarkdownChanged));

		public static readonly DependencyProperty StyleResourceModeProperty =
			DependencyProperty.Register(nameof(StyleResourceMode), typeof(StyleResourceMode), typeof(MarkdownViewer),
			new PropertyMetadata(StyleResourceMode.Static, OnStyleResourceModeChanged));

		public static readonly DependencyProperty ImageMaxDecodeWidthProperty =
			DependencyProperty.Register(nameof(ImageMaxDecodeWidth), typeof(int), typeof(MarkdownViewer),
				new PropertyMetadata(0, OnMarkdownChanged));

		public MarkdownPipeline Pipeline
		{
			get => (MarkdownPipeline)GetValue(PipelineProperty);
			set => SetValue(PipelineProperty, value);
		}

		public string Markdown
		{
			get => (string)GetValue(MarkdownProperty);
			set => SetValue(MarkdownProperty, value);
		}

		public StyleResourceMode StyleResourceMode
		{
			get => (StyleResourceMode)GetValue(StyleResourceModeProperty);
			set => SetValue(StyleResourceModeProperty, value);
		}

		public int ImageMaxDecodeWidth
		{
			get => (int)GetValue(ImageMaxDecodeWidthProperty);
			set => SetValue(ImageMaxDecodeWidthProperty, value);
		}

		public MarkdownViewer()
		{
			VirtualizingPanel.SetScrollUnit(this, ScrollUnit.Pixel);
			VirtualizingPanel.SetIsVirtualizing(this, true);
			VirtualizingPanel.SetVirtualizationMode(this, VirtualizationMode.Recycling);
			ScrollViewer.SetCanContentScroll(this, true);

			var factory = new FrameworkElementFactory(typeof(VirtualizingStackPanel));
			ItemsPanel = new ItemsPanelTemplate(factory);

			var template = new ControlTemplate(typeof(ItemsControl));
			var scrollViewer = new FrameworkElementFactory(typeof(ScrollViewer));
			scrollViewer.SetValue(ScrollViewer.PaddingProperty, new Thickness(15));
			scrollViewer.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);

			var presenter = new FrameworkElementFactory(typeof(ItemsPresenter));
			scrollViewer.AppendChild(presenter);
			template.VisualTree = scrollViewer;
			Template = template;
		}

		private static void OnMarkdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is not MarkdownViewer viewer)
				return;

			if (e.NewValue is not string)
				return;

			viewer.RenderMarkdown((string)e.NewValue);
		}

		private static void OnStyleResourceModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if(d is MarkdownViewer viewer)
			{
				viewer.RenderMarkdown(viewer.Markdown);
			}
		}

		private void RenderMarkdown(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
			{
				ItemsSource = null;
				return;
			}

			var pipeline = Pipeline ?? _defaultPipeline;

			var optimalImageDecodeWidth = CalculateOptimalDecodeWidth();
			var document = Markdig.Markdown.Parse(text, pipeline);

			var renderer = new WpfVirtualizingRenderer(this, StyleResourceMode)
			{
				OptimalImageDecodeWidth = optimalImageDecodeWidth
			};

			pipeline.Setup(renderer);

			var elements = (List<UIElement>)renderer.Render(document);
			ItemsSource = elements;
		}

		private int CalculateOptimalDecodeWidth()
		{
			if(ImageMaxDecodeWidth > 0)
			{
				return ImageMaxDecodeWidth;
			}

			var optimalWidth = SystemParameters.PrimaryScreenWidth > 0 ? SystemParameters.PrimaryScreenWidth : 1920;

			if(!double.IsPositiveInfinity(this.MaxWidth) && this.MaxWidth > 0)
			{
				return (int)this.MaxWidth;
			}

			var window = Window.GetWindow(this);
			if(window != null)
			{
				if(!double.IsPositiveInfinity(window.MaxWidth) && window.MaxWidth > 0)
				{
					optimalWidth = Math.Min(optimalWidth, window.MaxWidth);
				}
				else if(window.ResizeMode == ResizeMode.NoResize || window.ResizeMode == ResizeMode.CanMinimize)
				{
					var staticWidth = window.Width > 0 ? window.Width : window.ActualWidth;
					if(staticWidth > 0)
					{
						optimalWidth = Math.Min(optimalWidth, staticWidth);
					}
				}
			}

			return (int)optimalWidth;
		}

	}
}
