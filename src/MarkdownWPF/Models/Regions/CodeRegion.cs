using MarkdownWPF.Models.Inlines;
using System.Reflection.Metadata.Ecma335;

namespace MarkdownWPF.Models.Regions
{
    public class CodeRegion : CollectionRegionBase<IInline>
    {
        public CodeRegion() : base()
        {
        }
    }
}
