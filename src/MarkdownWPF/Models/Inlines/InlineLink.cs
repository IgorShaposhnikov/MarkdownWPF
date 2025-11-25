namespace MarkdownWPF.Models.Inlines
{
    public class InlineLink : IInline
    {
        public string Text { get; }
        public string? Url { get; }
        /// <summary>
        /// Is url result an image?
        /// </summary>
        public bool IsImage { get; }
        /// <summary>
        /// The result of concantention parent style blocks.
        /// </summary>
        public IStyleableInline? ParentInlinesStyle { get; set; }

        public InlineLink(string text, string url, bool isImage, IStyleableInline? parentInlinesStyles = null)
        {
            Text = text;
            Url = url;
            IsImage = isImage;
            ParentInlinesStyle = parentInlinesStyles;
        }
    }

    public class InlineImage : InlineLink
    {
        /// <summary>
        /// Url for clickable image
        /// </summary>
        public string? AdditionalUrl { get; }

        public double Width { get; } = double.NaN;
        public double Height { get; } = double.NaN;

        public InlineImage(string text, string url, string? addtionalUrl = null, IStyleableInline? parentInlinesStyles = null) : base(text, url, true, parentInlinesStyles)
        {
            AdditionalUrl = addtionalUrl;
        }

        public InlineImage(string text, string url, double width, double height, string ? addtionalUrl = null, IStyleableInline? parentInlinesStyles = null) : base(text, url, true, parentInlinesStyles)
        {
            AdditionalUrl = addtionalUrl;
            Width = width;
            Height = height;
        }
    }
}
