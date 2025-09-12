namespace MarkdownWPF.Models.Inlines
{
    public interface IStyleableInline
    {
        public EmphasisWeight Weight { get; }
        public IList<EmphasisDecorations> Decorations { get; }
        public EmphasisStyle Style { get; }
        public IList<EmphasisTypography> TypographyElements { get; }
        public bool HasHighlight { get; }
    }
}
