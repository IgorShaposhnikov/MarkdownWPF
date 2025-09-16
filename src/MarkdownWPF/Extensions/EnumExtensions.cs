using MarkdownWPF.Models.Inlines;

namespace MarkdownWPF.Extensions
{
    public static class EnumExtensions
    {
        public static EmphasisWeight GetByPriority(this EmphasisWeight weight, EmphasisWeight weight1) 
        {
            return (EmphasisWeight)Math.Max((byte)weight, (byte)weight1);
        }

        public static EmphasisStyle GetByPriority(this EmphasisStyle style, EmphasisStyle style1)
        {
            return (EmphasisStyle)Math.Max((byte)style, (byte)style1);
        }
    }
}
