using Markdig.Syntax;

namespace MarkdownWPF.Html
{
    public static class MarkdownBuilderExtensions
    {
        public static MarkdownParseBuilder UseHtmlParsing(this MarkdownParseBuilder builder) 
        {
            builder.AddExtension(typeof(HtmlBlock), (htmlBlock) => 
            {
                var html = htmlBlock as HtmlBlock;

                var htmlConverter = new HtmlConverter();
                return htmlConverter.Convert(html.Lines.ToString());
            });
            return builder;
        }  
    }
}
