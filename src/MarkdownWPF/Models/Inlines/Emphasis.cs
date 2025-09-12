namespace MarkdownWPF.Models.Inlines
{
    public class Emphasis : InlineContainer, IStyleableInline
    {
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
            bool hasHighlight = default,
            IList<IInline> inlines = null)
        {
            Text = text;
            TypographyElements = typographyElements;
            Weight = type;
            Decorations = decoration;
            Style = style;
            HasHighlight = hasHighlight;
            Inlines = inlines ?? [];
            IsTextOnly = inlines == null;
        }
    }
}
