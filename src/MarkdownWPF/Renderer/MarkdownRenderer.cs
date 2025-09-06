using MarkdownWPF.Models;
using System.Windows;
using System.Windows.Documents;

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
            }

            return run;
        }
    }
}
