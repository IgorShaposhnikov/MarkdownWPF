using Markdig.Syntax;
using MarkdownWPF.Models.Regions;

namespace MarkdownWPF
{
    public class MarkdownParseBuilder
    {
        private readonly Dictionary<Type, Func<IMarkdownObject, IEnumerable<IRegion>>> _extensions = new();

        public MarkdownParser Build() 
        {
            return new MarkdownParser(_extensions);
        }

        public void AddExtension(Type type, Func<IMarkdownObject, IEnumerable<IRegion>> extension) 
        {
            _extensions.Add(type, extension);
        }
    }
}
