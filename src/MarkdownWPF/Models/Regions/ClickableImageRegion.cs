using MarkdownWPF.Models.Inlines;

namespace MarkdownWPF.Models.Regions
{
    public class ClickableImageRegion : ImageRegion
    {
        public string? AddtionalUrl { get; }

        public ClickableImageRegion(Image image) : base(image)
        {
            AddtionalUrl = image.AdditionalUrl;
        }
    }
}
