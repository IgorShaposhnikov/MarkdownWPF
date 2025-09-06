using MarkdownWPF.Models;
using System.Windows.Documents;

namespace MarkdownWPF.Renderer
{
    public interface IMarkdownRenderer
    {
        public Inline RenderInline(IInline inline);
    }
}
