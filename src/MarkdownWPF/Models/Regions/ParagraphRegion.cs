using MarkdownWPF.Models.Inlines;

namespace MarkdownWPF.Models.Regions
{
    public sealed class ParagraphRegion : CollectionRegionBase<IInline>
    {
        public ParagraphRegion(IEnumerable<IInline> inlines)
        {
            Value = inlines.ToList();
        }
    }
}
