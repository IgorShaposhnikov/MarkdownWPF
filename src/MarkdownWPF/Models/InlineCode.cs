namespace MarkdownWPF.Models
{
    public class InlineCode : IInline
    {
        public string Text { get; }

        public InlineCode(string text)
        {
            Text = text;
        }
    }
}
