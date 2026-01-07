using Markdig.Syntax;
using MarkdownWPF.Models;

namespace MarkdownWPF
{
    public class MarkdownParseBuilder
    {
        private readonly Dictionary<Type, Func<IMarkdownObject, IEnumerable<IMarkdownElement>>> _extensions = new();

        public MarkdownParser Build() 
        {
            return new MarkdownParser(_extensions);
        }

        public void AddExtension(Type type, Func<IMarkdownObject, IEnumerable<IMarkdownElement>> extension) 
        {
            _extensions.Add(type, extension);
        }
    }
}
