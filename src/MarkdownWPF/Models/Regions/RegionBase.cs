namespace MarkdownWPF.Models.Regions
{
    public abstract class RegionBase<T> : IMarkdownElement
        where T : IMarkdownElement
    {
        public List<T> Elements { get; protected set; } = [];
    }
}
