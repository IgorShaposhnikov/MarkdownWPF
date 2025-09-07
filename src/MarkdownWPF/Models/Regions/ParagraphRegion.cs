namespace MarkdownWPF.Models.Regions
{
    public sealed class ParagraphRegion : RegionBase<IInline>
    {
        public ParagraphRegion(IEnumerable<IInline> inlines)
        {
            Elements = inlines.ToList();
        }
    }
}
