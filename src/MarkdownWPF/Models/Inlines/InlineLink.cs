namespace MarkdownWPF.Models.Inlines
{
    public interface IInlineLink : IInline
    {
        public string? Url { get; }
        /// <summary>
        /// Is url result an image?
        /// </summary>
        public bool IsImage { get; }
        /// <summary>
        /// The result of concantention parent style blocks.
        /// </summary>
        public IStyleableInline? ParentInlinesStyle { get; set; }
    }

    public class InlineLink : InlineContainer, IInlineLink
    {
        public string? Url { get; }
        /// <summary>
        /// Is url result an image?
        /// </summary>
        public bool IsImage { get; }
        /// <summary>
        /// The result of concantention parent style blocks.
        /// </summary>
        public IStyleableInline? ParentInlinesStyle { get; set; }

        public InlineLink(string url, bool isImage, IList<IInline> inlines, IStyleableInline? parentInlinesStyles = null)
        {
            Inlines = inlines;
        }

        public InlineLink(string text, string url, bool isImage, IStyleableInline? parentInlinesStyles = null)
        {
            Inlines = [new Paragraph(text)];
        }
    }

    public class InlineImage : IInlineLink
    {
        /// <summary>
        /// Url for clickable image
        /// </summary>
        public string? AdditionalUrl { get; }

        public double Width { get; } = double.NaN;
        public double Height { get; } = double.NaN;

        public string Text { get; }
        public string? Url { get; }
        /// <summary>
        /// Is url result an image?
        /// </summary>
        public bool IsImage => true;
        /// <summary>
        /// The result of concantention parent style blocks.
        /// </summary>
        public IStyleableInline? ParentInlinesStyle { get; set; }

        public InlineImage(string text, string url, string? addtionalUrl = null, IStyleableInline? parentInlinesStyles = null)
        {
            Text = text;
            Url = url;
            ParentInlinesStyle = parentInlinesStyles;
            AdditionalUrl = addtionalUrl;
        }

        public InlineImage(string text, string url, double width, double height, string? addtionalUrl = null, IStyleableInline? parentInlinesStyles = null)
        {
            Text = text;
            Url = url;
            ParentInlinesStyle = parentInlinesStyles;
            AdditionalUrl = addtionalUrl;
            Width = width;
            Height = height;
        }
    }
}
