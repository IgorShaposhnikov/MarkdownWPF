using MarkdownWPF.Models;
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

            if (inline is Emphasis emphasis)
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

                Console.WriteLine("All Text: " + run.Text);
            }

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
