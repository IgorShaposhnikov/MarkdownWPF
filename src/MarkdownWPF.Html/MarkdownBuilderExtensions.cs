using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownWPF.Extensions;

namespace MarkdownWPF.Html
{
    public static class MarkdownBuilderExtensions
    {
        public static MarkdownParseBuilder UseHtmlParsing(this MarkdownParseBuilder builder) 
        {
            var htmlConverter = new HtmlConverter();
            builder.AddExtension(typeof(HtmlBlock), (htmlBlock) => 
            {
                var html = htmlBlock as HtmlBlock;
                return htmlConverter.Convert(html.Lines.ToString());
            });

            builder.AddExtension(typeof(HtmlInline), (htmlInline) => 
            {
                var html = (htmlInline as HtmlInline).GetFullHtml();
                return htmlConverter.Convert(html.ToString());
            });

            return builder;
        }  
    }
}
