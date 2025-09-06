namespace MarkdownWPF.Models
{
    public enum EmphasisWeight
    {
        Normal,
        Bold,
    }

    public enum EmphasisStyle 
    {
        Normal,
        Italic
    }

    public enum EmphasisDecorations 
    {
        None,
        Baseline,
        Strikethrough,
        Overline,
        Underline,
    }

    public class Emphasis : IInline
    {
        public string Text { get; set; }

        public Emphasis(string text, EmphasisWeight type = default, EmphasisDecorations decoration = default, EmphasisStyle style = default)
        {
            Text = text;
            Weight = type;
            Decoration = decoration;
            Style = style;
        }

        public EmphasisWeight Weight { get; set; }
        public EmphasisDecorations Decoration { get; set; }
        public EmphasisStyle Style { get; set; }
    }
}
