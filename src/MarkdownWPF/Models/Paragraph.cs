namespace MarkdownWPF.Models
{
    public class Paragraph : IInline
    {
        public string Text { get; }

        public Paragraph(string value)
        {
            Text = value;
        }
    }
}
