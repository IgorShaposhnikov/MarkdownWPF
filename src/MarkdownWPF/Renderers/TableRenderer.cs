using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MdTable = Markdig.Extensions.Tables.Table;
using MdTableRow = Markdig.Extensions.Tables.TableRow;
using MdTableCell = Markdig.Extensions.Tables.TableCell;

namespace MarkdownWPF.Renderers
{
    public class TableContext 
    {
        public Grid Grid { get; set; } = new Grid();
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public bool IsInHeader { get; set; }
        public MdTable? CurrentTable { get; set; }
    }

    public class TableRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, MdTable>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, MdTable obj)
        {
            var grid = new Grid { Margin = new Thickness(0, 10, 0, 15), HorizontalAlignment = HorizontalAlignment.Stretch };
            for (int i = 0; i < obj.ColumnDefinitions.Count; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            if (renderer.CurrentContext == null) renderer.RootElements.Add(grid);
            else if (renderer.CurrentContext is StackPanel sp) sp.Children.Add(grid);

            var ctx = new TableContext { Grid = grid, RowIndex = -1, ColIndex = 0, IsInHeader = false, CurrentTable = obj };
            renderer.Push(ctx);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
    }

    public class TableRowRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, MdTableRow>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, MdTableRow obj)
        {
            if (renderer.CurrentContext is TableContext ctx)
            {
                ctx.Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                ctx.RowIndex++;
                ctx.ColIndex = 0; 
                ctx.IsInHeader = obj.IsHeader;
                renderer.WriteChildren(obj);
            }
        }
    }

    public class TableCellRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, MdTableCell>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, MdTableCell obj)
        {
            if (renderer.CurrentContext is TableContext ctx)
            {
                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(223, 226, 229)),
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(10, 6, 10, 6),
                    Background = (ctx.RowIndex % 2 == 0) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(246, 248, 250))
                };
                if (ctx.IsInHeader) border.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));

                Grid.SetRow(border, ctx.RowIndex);
                Grid.SetColumn(border, ctx.ColIndex);
                ctx.Grid.Children.Add(border);

                var tb = new TextBlock { VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap };
                if (ctx.IsInHeader) tb.FontWeight = FontWeights.Bold;

                if (ctx.CurrentTable != null && ctx.ColIndex < ctx.CurrentTable.ColumnDefinitions.Count)
                {
                    var alignment = ctx.CurrentTable.ColumnDefinitions[ctx.ColIndex].Alignment;
                    if (alignment.HasValue)
                    {
                        switch (alignment.Value)
                        {
                            case Markdig.Extensions.Tables.TableColumnAlign.Left: tb.TextAlignment = TextAlignment.Left; break;
                            case Markdig.Extensions.Tables.TableColumnAlign.Center: tb.TextAlignment = TextAlignment.Center; break;
                            case Markdig.Extensions.Tables.TableColumnAlign.Right: tb.TextAlignment = TextAlignment.Right; break;
                        }
                    }
                }

                border.Child = tb;
                renderer.Push(tb);
                foreach (var block in obj)
                {
                    if (block is ParagraphBlock paragraph && paragraph.Inline != null) renderer.WriteChildren(paragraph.Inline);
                    else renderer.Write(block);
                }
                renderer.Pop();
                ctx.ColIndex++;
            }
        }
    }
}