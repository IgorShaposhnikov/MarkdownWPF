using MarkdownWPF.Models.Inlines;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MarkdownWPF.Renderer
{
    public class MarkdownRenderer : IMarkdownRenderer
    {
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
                var hyperLink = new Hyperlink();
                hyperLink.Click += (s, e) =>
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo() { FileName = inlineLink.Url, UseShellExecute = true });
                    }
                    catch
                    {

                    }
                };

                if (inlineLink.ParentInlinesStyle != null)
                {
                    run = GenerateStyleableBlock(run, inlineLink.ParentInlinesStyle);
                }

                hyperLink.Inlines.Add(run);
                return hyperLink;
            }


            return run;
        }

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

            Debug.WriteLine("All Text: " + run.Text);

            return run;
        }

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
