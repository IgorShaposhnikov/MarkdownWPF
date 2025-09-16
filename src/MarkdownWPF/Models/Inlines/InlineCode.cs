namespace MarkdownWPF.Models.Inlines
{
    public class InlineCode : IInline
    {
        public string Text { get; }
        /// <summary>
        /// The result of concantention parent style regions.
        /// </summary>
        public IStyleableInline? ParentInlinesStyle { get; set; }

        public InlineCode(string text, IStyleableInline? parentInlinesStyle = null)
        {
            Text = text;
            ParentInlinesStyle = parentInlinesStyle;
        }
    }
}
