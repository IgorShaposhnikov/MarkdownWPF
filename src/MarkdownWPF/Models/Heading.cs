namespace MarkdownWPF.Models
{
    public class Heading : IMarkdownElement
    {
        public IEnumerable<IInline> Value { get; }
        public int Level { get; }

        public Heading(IEnumerable<IInline> value, int level)
        {
            Value = value;
            Level = level;
        }

        public override string ToString()
        {
            return string.Join("", Value);
        }
    }
}
