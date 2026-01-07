using MarkdownWPF.Models.Inlines;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MarkdownWPF.Renderer
{
    /// <summary>
    /// Renders Markdown content to WPF Inline elements for display in RichTextBox or TextBlock.
    /// Implements the IMarkdownRenderer interface to provide Markdown-to-WPF conversion.
    /// </summary>
    public class MarkdownRenderer : IMarkdownRenderer
    {
        /// <summary>
        /// Converts a single inline Markdown element to its corresponding WPF Inline representation.
        /// </summary>
        /// <param name="inline">The Markdown inline element to render.</param>
        /// <returns>A WPF Inline element ready for display in a text container.</returns>
        public Inline RenderInline(IInline inline)
        {
            var run = new Run
            {
                Text = inline.Text
            };

            if (inline is IStyleableInline emphasis)
            {
                run = GenerateStyleableBlock(run, emphasis);
            }
            else if (inline is InlineCode inlineCode)
            {
               if (inlineCode.ParentInlinesStyle != null)
                {
                    run = GenerateStyleableBlock(run, inlineCode.ParentInlinesStyle);
                }
                // Visible "padding"
                run.Text = $" {run.Text} ";
                // TODO: Need add a way to styling blocks;
                run.Background = new BrushConverter().ConvertFrom("#f9f2f4") as SolidColorBrush;
                run.Foreground = new BrushConverter().ConvertFrom("#c7254e") as SolidColorBrush;
            }
            else if (inline is InlineLink inlineLink)
            {
                return RenderInlineLink(inlineLink);
            }

            return run;
        }

        /// <summary>
        /// Renders a container holding multiple inline elements, handling special cases for nested elements.
        /// </summary>
        /// <param name="container">The container with inline elements to render.</param>
        /// <returns>A collection of WPF Inline elements representing the container's contents.</returns>
        public IEnumerable<Inline> RenderInlineContainer(InlineContainer container)
        {
            if (container.IsTextOnly)
            {
                return [RenderInline(container)];
            }

            var res = new List<IInline>();
            foreach (var i in container.Inlines)
            {
                if (i is InlineLink inlineLink)
                {
                    inlineLink.ParentInlinesStyle = (Emphasis)container;
                    res.Add(inlineLink);
                }
                else if (i is InlineCode inlineCode)
                {
                    inlineCode.ParentInlinesStyle = (Emphasis)container;
                    res.Add(inlineCode);
                }
            }

            return res.Select(RenderInline);
        }

        /// <summary>
        /// Converts a collection of Markdown inline elements to basic WPF Run elements.
        /// Primarily used for simple text rendering without complex formatting.
        /// </summary>
        /// <param name="inlines">Collection of Markdown inline elements to convert.</param>
        /// <returns>A sequence of WPF Run elements with applied styling.</returns>
        public IEnumerable<Run> InlinesToRuns(IEnumerable<IInline> inlines) 
        {
            foreach (var inline in inlines) 
            {
                if (inline is IStyleableInline emphasis)
                {
                    yield return GenerateStyleableBlock(new Run(emphasis.Text), emphasis);
                }
                else 
                {
                    yield return new Run(inline.Text);
                }
            }
        }

        /// <summary>
        /// Renders a Markdown link as a clickable WPF Hyperlink with optional styling inheritance.
        /// </summary>
        /// <param name="inlineLink">The Markdown link element containing URL and display text.</param>
        /// <returns>A WPF Hyperlink element with click event handling for navigation.</returns>
        private Hyperlink RenderInlineLink(InlineLink inlineLink) 
        {
            var hyperLink = new Hyperlink();

            var run = new Run
            {
                Text = inlineLink.Text
            };

            hyperLink.Click += (s, e) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo() { FileName = inlineLink.Url, UseShellExecute = true });
                }
                catch { }
            };

            if (inlineLink.ParentInlinesStyle != null)
            {
                run = GenerateStyleableBlock(run, inlineLink.ParentInlinesStyle);
            }
            else if (inlineLink.Inlines != null)
            {
                if (inlineLink.Inlines.Count == 1 && inlineLink.Inlines.First().GetType() is IStyleableInline styleable)
                {
                    run = GenerateStyleableBlock(run, styleable);
                    hyperLink.Inlines.Add(run);
                    return hyperLink;
                }

                hyperLink.Inlines.AddRange(InlinesToRuns(inlineLink.Inlines));
                return hyperLink;
            }

            return hyperLink;
        }

        /// <summary>
        /// Applies text styling properties to a Run element based on emphasis configuration.
        /// Supports combinations of italic, bold, text decorations, and highlighting.
        /// </summary>
        /// <param name="run">The base Run element to style.</param>
        /// <param name="emphasis">The styling configuration to apply.</param>
        /// <returns>The styled Run element with all specified emphasis properties applied.</returns>
        private Run GenerateStyleableBlock(Run run, IStyleableInline emphasis)
        {
            if (emphasis.Style == EmphasisStyle.Italic)
            {
                run.FontStyle = FontStyles.Italic;
            }

            if (emphasis.Weight == EmphasisWeight.Bold)
            {
                run.FontWeight = FontWeights.Bold;
            }

            if (emphasis.Decorations.Count > 0)
            {
                TextDecorationCollection decorations = new();

                foreach (var decoration in emphasis.Decorations)
                {
                    decorations.Add(GetDecorationByType(decoration));
                }

                run.TextDecorations = decorations;
            }

            if (emphasis.HasHighlight)
            {
                run.Background = new SolidColorBrush(Colors.LightCyan);
            }

            return run;
        }

        /// <summary>
        /// Maps Markdown emphasis decoration types to WPF TextDecoration equivalents.
        /// </summary>
        /// <param name="decoration">The Markdown decoration type to convert.</param>
        /// <returns>The corresponding WPF TextDecoration object.</returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when an unsupported decoration type is provided.
        /// </exception>
        private TextDecorationCollection GetDecorationByType(EmphasisDecorations decoration)
        {
            return decoration switch
            {
                EmphasisDecorations.Baseline => TextDecorations.Baseline,
                EmphasisDecorations.Strikethrough => TextDecorations.Strikethrough,
                EmphasisDecorations.Overline => TextDecorations.OverLine,
                EmphasisDecorations.Underline => TextDecorations.Underline,
                _ => throw new NotImplementedException()
            };
        }
    }
}
