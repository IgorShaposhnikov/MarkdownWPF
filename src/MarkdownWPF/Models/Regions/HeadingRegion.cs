namespace MarkdownWPF.Models.Regions
{
    public class HeadingRegion : RegionBase<IInline>
    {
        public int Level { get; }

        public HeadingRegion(IEnumerable<IInline> elements, int level)
        {
            Elements = elements.ToList();
            Level = level;
        }

        public override string ToString()
        {
            return string.Join("", Elements);
        }
    }
}
