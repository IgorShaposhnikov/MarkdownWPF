using MarkdownWPF.Models.Inlines;

namespace MarkdownWPF.Models.Regions
{
    public class ImageRegion : RegionBase<InlineLink>
    {
        public string Title { get; }

        public ImageRegion(Image image)
        {
            Value = image;
            Title = image.Text;
        }
    }
}
