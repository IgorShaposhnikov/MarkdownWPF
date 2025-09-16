using MarkdownWPF.Extensions;

namespace MarkdownWPF.Models.Inlines
{
    public class Emphasis : InlineContainer, IStyleableInline
    {
        public EmphasisWeight Weight { get; }
        public EmphasisStyle Style { get; }
        public ISet<EmphasisDecorations> Decorations { get; } = new HashSet<EmphasisDecorations>();
        public ISet<EmphasisTypography> TypographyElements { get; } = new HashSet<EmphasisTypography>();
        public bool HasHighlight { get; }

        public Emphasis(string text,
            ISet<EmphasisTypography> typographyElements,
            ISet<EmphasisDecorations> decoration,
            EmphasisWeight type = default,
            EmphasisStyle style = default,
            bool hasHighlight = default,
            IList<IInline>? inlines = null)
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

        public Emphasis Merge(Emphasis emphasis)
        {
            var mergedTypographies = new HashSet<EmphasisTypography>();
            var mergedDecorations = new HashSet<EmphasisDecorations>();

            TypographyElements.Select(mergedTypographies.Add);
            emphasis.TypographyElements.Select(mergedTypographies.Add);

            Decorations.Select(mergedDecorations.Add);
            emphasis.Decorations.Select(mergedDecorations.Add);

            return new Emphasis(
                $"{Text}{emphasis.Text}",
                mergedTypographies,
                mergedDecorations,
                Weight.GetByPriority(emphasis.Weight),
                Style.GetByPriority(emphasis.Style)
                );
        }
    }
}
