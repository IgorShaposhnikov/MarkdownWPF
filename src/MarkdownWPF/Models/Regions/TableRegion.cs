namespace MarkdownWPF.Models.Regions
{
    public class TableRegion : CollectionRegionBase<TableRowRegion> { }

    public class TableRowRegion : CollectionRegionBase<TableCellRegion> { }

    public class TableCellRegion : CollectionRegionBase<IRegion> { }
}
