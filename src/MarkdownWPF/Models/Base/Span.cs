namespace MarkdownWPF.Models.Base
{
    public readonly struct Span
    {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public int Size { get; }

        public Span(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Size = endIndex - startIndex;
        }
    }
}
