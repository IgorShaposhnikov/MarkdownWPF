using Markdig.Renderers;
using Markdig.Syntax;
using MarkdownWPF.Extensions;
using MarkdownWPF.Renderers;
using System.Windows;

namespace MarkdownWPF
{
    public class WpfVirtualizingRenderer : RendererBase
    {
        public List<UIElement> RootElements { get; } = new();
        private readonly Stack<object> _contextStack = new();
        public FrameworkElement? ContextElement { get; }
        public StyleResourceMode StyleResourceMode { get; }
        public int OptimalImageDecodeWidth { get; set; } = 1920;

        // Кэш для стилей
        private readonly Dictionary<string, Style?> _styleCache = new();

        public WpfVirtualizingRenderer(FrameworkElement? frameworkElement = null, StyleResourceMode styleResourceMode = StyleResourceMode.Static)
        {
            ContextElement = frameworkElement;
            StyleResourceMode = styleResourceMode;

            ObjectRenderers.Add(new HeadingRenderer());
            ObjectRenderers.Add(new ParagraphRenderer());
            ObjectRenderers.Add(new CodeBlockRenderer());
            ObjectRenderers.Add(new ListRenderer());
            ObjectRenderers.Add(new ListItemRenderer());
            ObjectRenderers.Add(new ThematicBreakRenderer());
            ObjectRenderers.Add(new BlockQuoteRenderer());

            ObjectRenderers.Add(new TableRenderer());
            ObjectRenderers.Add(new TableRowRenderer());
            ObjectRenderers.Add(new TableCellRenderer());

            ObjectRenderers.Add(new EmphasisRenderer());
            ObjectRenderers.Add(new LinkInlineRenderer());
            ObjectRenderers.Add(new LiteralInlineRenderer());
            ObjectRenderers.Add(new CodeInlineRenderer());
            ObjectRenderers.Add(new LineBreakInlineRenderer());
            ObjectRenderers.Add(new HtmlInlineRenderer());
            ObjectRenderers.Add(new HtmlBlockRenderer());
        }

        public override object Render(MarkdownObject markdownObject)
        {
            Write(markdownObject);
            return RootElements;
        }

        // For block elements (Border, TextBlock, Grid)
        public void ApplyStyle(FrameworkElement element, string styleKey)
        {
            if (StyleResourceMode == StyleResourceMode.Dynamic)
            {
                element.SetResourceReference(FrameworkElement.StyleProperty, styleKey);
                return;
            }

            if (!_styleCache.TryGetValue(styleKey, out var style))
            {
                style = ContextElement?.TryFindResource(styleKey) as Style;
                _styleCache[styleKey] = style;
            }
            if (style != null) element.Style = style;
        }

        // For Inline elements (Run, Span, Hyperlink)
        public void ApplyStyle(FrameworkContentElement element, string styleKey)
        {
            if (StyleResourceMode == StyleResourceMode.Dynamic)
            {
                element.SetResourceReference(FrameworkContentElement.StyleProperty, styleKey);
                return;
            }

            if (!_styleCache.TryGetValue(styleKey, out var style))
            {
                style = ContextElement?.TryFindResource(styleKey) as Style;
                _styleCache[styleKey] = style;
            }
            if (style != null) element.Style = style;
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

        public void HandleLastChildMargin(FrameworkElement element, Markdig.Syntax.Block block)
        {
            if (block.Parent != null && block.Parent[block.Parent.Count - 1] == block)
            {
                MarkdownProperties.SetIsLastChild(element, true);
            }
        }
    }
}