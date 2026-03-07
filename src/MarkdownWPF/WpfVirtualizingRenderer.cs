using Markdig.Renderers;
using Markdig.Syntax;
using MarkdownWPF.Renderers;
using System.Windows;

namespace MarkdownWPF
{
    public class WpfVirtualizingRenderer : RendererBase
    {
        public List<UIElement> RootElements { get; } = new();
        private readonly Stack<object> _contextStack = new();

        public WpfVirtualizingRenderer()
        {
            ObjectRenderers.Add(new HeadingRenderer());
            ObjectRenderers.Add(new ParagraphRenderer());
            ObjectRenderers.Add(new CodeBlockRenderer());
            ObjectRenderers.Add(new ListRenderer());
            ObjectRenderers.Add(new ListItemRenderer());
            ObjectRenderers.Add(new ThematicBreakRenderer());

            ObjectRenderers.Add(new TableRenderer());
            ObjectRenderers.Add(new TableRowRenderer());
            ObjectRenderers.Add(new TableCellRenderer());

            ObjectRenderers.Add(new EmphasisRenderer());
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new LiteralInlineRenderer());
            ObjectRenderers.Add(new CodeInlineRenderer());
            ObjectRenderers.Add(new LineBreakInlineRenderer());
        }

        public override object Render(MarkdownObject markdownObject)
        {
            Write(markdownObject);
            return RootElements;
        }

        public void Push(object context) => _contextStack.Push(context);
        public void Pop() => _contextStack.Pop();
        public object? CurrentContext => _contextStack.Count > 0 ? _contextStack.Peek() : null;

        public void WriteLeafInline(LeafBlock leafBlock)
        {
            if (leafBlock.Inline != null)
            {
                WriteChildren(leafBlock.Inline);
            }
        }
    }
}