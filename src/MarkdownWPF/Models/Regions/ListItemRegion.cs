namespace MarkdownWPF.Models.Regions
{
    public class ListItemRegion : CollectionRegionBase<IRegion>
    {
        public char? BulletType { get; set; }
        public string? OrderedStart { get; set; }
        public char? OrderedDelimiter { get; set; }
        public bool IsOrdered { get; set; }
        public int? Order { get; set; }

        public ListItemRegion(IList<IRegion> regions)
        {
            Value = regions;
        }
    }
}
