namespace MarkdownWPF.Models
{
    public sealed class ParagraphBlock : IMarkdownElement
    {
        public IEnumerable<IInline> Inlines { get; set; }

        public ParagraphBlock(IEnumerable<IInline> inlines)
        {
            Inlines = inlines;
        }
    }
}
