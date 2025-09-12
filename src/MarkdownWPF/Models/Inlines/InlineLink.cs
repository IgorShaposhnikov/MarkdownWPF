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
        public IStyleableInline? ParentInlinesStyle { get; }

        public InlineLink(string text, string url, bool isImage, IStyleableInline? parentInlinesStyles = null)
        {
            Text = text;
            Url = url;
            IsImage = isImage;
            ParentInlinesStyle = parentInlinesStyles;
        }
    }
}
