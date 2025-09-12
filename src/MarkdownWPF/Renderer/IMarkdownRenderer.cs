using MarkdownWPF.Models.Inlines;
using System.Windows.Documents;

namespace MarkdownWPF.Renderer
{
    public interface IMarkdownRenderer
    {
        public Inline RenderInline(IInline inline);
        public IEnumerable<Inline> RenderInlineContainer(InlineContainer container);
    }
}
