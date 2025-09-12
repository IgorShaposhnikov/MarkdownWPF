using MarkdownWPF.Models.Base;

namespace MarkdownWPF.Models.Inlines
{
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
}
