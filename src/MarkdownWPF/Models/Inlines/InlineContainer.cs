namespace MarkdownWPF.Models.Inlines
{
    public abstract class InlineContainer : IInline
    {
        /// <summary>
        /// Is Emphasis contains only.
        /// </summary>
        public bool IsTextOnly { get; protected set; }
        /// <summary>
        /// Has elements if content is not only text
        /// </summary>
        public string? Text { get; protected set; }
        public IList<IInline> Inlines { get; protected set; }
    }
}
