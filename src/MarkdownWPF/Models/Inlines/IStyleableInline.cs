using MarkdownWPF.Models.Base;

namespace MarkdownWPF.Models.Inlines
{
    public interface IStyleableInline : IInline
    {
        public EmphasisWeight Weight { get; }
        public ISet<EmphasisDecorations> Decorations { get; }
        public ISet<EmphasisTypography> TypographyElements { get; }
        public EmphasisStyle Style { get; }
        public bool HasHighlight { get; }
    }
}
