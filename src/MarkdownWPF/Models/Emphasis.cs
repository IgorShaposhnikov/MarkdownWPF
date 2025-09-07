using MarkdownWPF.Models.Base;

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
        Baseline,
        Strikethrough,
        Overline,
        Underline,
    }

    public enum EmphasisTypographyVariants
    {
        Normal,
        Superscript,
        Subscript
    }

    public readonly struct EmphasisTypography
    {
        public EmphasisTypographyVariants Variant { get; } = EmphasisTypographyVariants.Normal;
        /// <summary>
        /// Substring variant location
        /// </summary>
        public Span TypographySpan { get; }

        public EmphasisTypography(Span typographySpan, EmphasisTypographyVariants variant)
        {
            TypographySpan = typographySpan;
            Variant = variant;
        }
    }

    public class Emphasis : IInline
    {
        public string Text { get; }
        public EmphasisWeight Weight { get; }
        public IList<EmphasisDecorations> Decorations { get; } = new List<EmphasisDecorations>();
        public EmphasisStyle Style { get; }
        public IList<EmphasisTypography> TypographyElements { get; } = new List<EmphasisTypography>();
        public bool HasHighlight { get; }

        public Emphasis(string text,
            IList<EmphasisTypography> typographyElements,
            IList<EmphasisDecorations> decoration,
            EmphasisWeight type = default,
            EmphasisStyle style = default,
            bool hasHighlight = default)
        {
            Text = text;
            TypographyElements = typographyElements;
            Weight = type;
            Decorations = decoration;
            Style = style;
            HasHighlight = hasHighlight;
        }
    }
}
