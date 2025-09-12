using MarkdownWPF.Models.Inlines;

namespace MarkdownWPF.Models.Regions
{
    public class ImageRegion : RegionBase<InlineLink>
    {
        public ImageRegion(InlineLink link)
        {
            Value = link;
        }
    }
}
