namespace MarkdownWPF.Models
{
    public interface IInline : IMarkdownElement
    {
        string Text { get; }
    }
}
