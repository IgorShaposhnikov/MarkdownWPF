using MarkdownWPF.Models.Inlines;

namespace MarkdownWPF.Models.Regions
{
    public class HeadingRegion : CollectionRegionBase<IInline>
    {
        public int Level { get; }

        public HeadingRegion(IEnumerable<IInline> elements, int level)
        {
            Value = elements.ToList();
            Level = level;
        }

        public override string ToString()
        {
            return string.Join("", Value);
        }
    }
}
