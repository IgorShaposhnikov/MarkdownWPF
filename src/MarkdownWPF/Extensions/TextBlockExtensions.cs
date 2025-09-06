using MarkdownWPF.Models;
using MarkdownWPF.Renderer;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MarkdownWPF.Extensions
{
    public static class TextBlockExtensions
    {
        #region Dependecy Properties


        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(IEnumerable), typeof(TextBlockExtensions), 
                new FrameworkPropertyMetadata(null, OnSourceChanged));

        public static IEnumerable GetSource(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty MarkdownRendererProperty =
            DependencyProperty.RegisterAttached("MarkdownRenderer", typeof(IMarkdownRenderer), typeof(TextBlockExtensions),
                new FrameworkPropertyMetadata(null, OnSourceChanged));

        public static IMarkdownRenderer GetMarkdownRenderer(DependencyObject obj)
        {
            return (IMarkdownRenderer)obj.GetValue(MarkdownRendererProperty);
        }

        public static void SetMarkdownRenderer(DependencyObject obj, IMarkdownRenderer value)
        {
            obj.SetValue(MarkdownRendererProperty, value);
        }


        #endregion Dependency Properties


        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBlock textBlock) return;

            textBlock.Inlines.Clear();

            var source = GetSource(textBlock);

            if (source == null) 
                return;

            // Get set value of IMarkdownRenderer
            var render = GetMarkdownRenderer(textBlock);
            
            if (render == null) 
                return;

            foreach (var item in source)
            {
                // Check the item is a IInline
                if (item is IInline mdElement)
                {
                    // Get renderable inline WPF element (like Run)
                    var inline = render.RenderInline(mdElement);
                    if (inline != null)
                    {
                        textBlock.Inlines.Add(inline);
                    }
                }
            }
        }
    }
}