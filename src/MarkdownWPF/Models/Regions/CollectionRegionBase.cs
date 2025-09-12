namespace MarkdownWPF.Models.Regions
{
    public abstract class CollectionRegionBase<T> : RegionBase<IList<T>>
        where T : IMarkdownElement
    {

    }

    public abstract class RegionBase<T> : IMarkdownElement
    {
        public T Value { get; protected set; }
    }
}
