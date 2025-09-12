namespace MarkdownWPF.Models.Inlines
{
    public interface IInline : IMarkdownElement
    {
        public string Text { get; }
    }
}
