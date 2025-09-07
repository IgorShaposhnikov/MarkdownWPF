namespace MarkdownWPF.Models
{
    public interface IInline : IMarkdownElement
    {
        public string Text { get; }
    }
}
