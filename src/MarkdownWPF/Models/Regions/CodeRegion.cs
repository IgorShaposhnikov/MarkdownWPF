namespace MarkdownWPF.Models.Regions
{
    public class CodeRegion : RegionBase<string>
    {
        public string LanguageName { get; } 

        public CodeRegion() : base()
        {

        }

        public CodeRegion(string code, string languageName) : base()
        {
            Value = code;
            LanguageName = languageName;
        }
    }
}
