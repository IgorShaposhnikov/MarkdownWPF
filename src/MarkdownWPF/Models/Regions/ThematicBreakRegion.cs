namespace MarkdownWPF.Models.Regions
{
    public class ThematicBreakRegion : CollectionRegionBase<IMarkdownElement>
    {
        public char ThematicChar { get; }
        public int ThematicCharCount { get; }

        public ThematicBreakRegion(char thematicChar, int thematicCharCount = 3)
        {
            ThematicChar = thematicChar;
            ThematicCharCount = thematicCharCount;
        }
    }
}
