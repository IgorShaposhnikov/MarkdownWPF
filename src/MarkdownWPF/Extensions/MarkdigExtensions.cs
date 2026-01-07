using Markdig.Syntax.Inlines;
using System.Text;

namespace MarkdownWPF.Extensions
{
    public static class MarkdigExtensions
    {
        public static string GetFullHtml(this HtmlInline htmlInline)
        {
            if (htmlInline == null) return string.Empty;

            var sb = new StringBuilder();
            Inline? current = htmlInline;

            while (current != null)
            {
                switch (current)
                {
                    case HtmlInline html:
                        sb.Append(html.Tag);
                        break;

                    case LiteralInline literal:
                        sb.Append(literal.Content.ToString());
                        break;

                    case ContainerInline container:
                        foreach (var child in container)
                        {
                            sb.Append(GetFullHtmlFromInline(child));
                        }
                        break;

                    default:
                        throw new NotSupportedException($"Unknown inline type {current.GetType()}");
                }

                current = current.NextSibling;
            }

            return sb.ToString();
        }

        private static string GetFullHtmlFromInline(Inline inline)
        {
            if (inline == null) return string.Empty;

            switch (inline)
            {
                case HtmlInline html:
                    return html.Tag;

                case LiteralInline literal:
                    return literal.Content.ToString();

                case ContainerInline container:
                    var sb = new StringBuilder();
                    foreach (var child in container)
                        sb.Append(GetFullHtmlFromInline(child));
                    return sb.ToString();

                default:
                    throw new NotSupportedException($"Unknown inline type {inline.GetType()}");
            }
        }
    }
}
