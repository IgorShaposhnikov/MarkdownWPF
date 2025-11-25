using HtmlAgilityPack;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownWPF.Models;
using MarkdownWPF.Models.Inlines;
using MarkdownWPF.Models.Regions;

namespace MarkdownWPF.Html
{
    internal class HtmlConverter
    {
        private HtmlNode _docNode;

        public IEnumerable<IRegion> Convert(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            _docNode = htmlDoc.DocumentNode;

            return Parse();
        }

        public IEnumerable<IRegion> Parse()
        {
            var regions = new List<IRegion>();

            foreach (var i in _docNode.ChildNodes)
            {
                if (i.NodeType != HtmlNodeType.Element)
                    continue;

                GetRegionByType(i, regions);
            }

            return regions;
        }

        public void GetRegionByType(HtmlNode htmlNode, IList<IRegion> regions)
        {
            if (htmlNode == null)
                return;

            Console.WriteLine(htmlNode.Name);

            switch (htmlNode.Name)
            {
                case "h1" or "h2" or "h3" or "h4" or "h5" or "h6":
                    {
                        regions.Add(ToHeaderRegion(htmlNode));
                        break;
                    }
                case "br":
                    break;
                case "hr":
                    regions.Add(new ThematicBreakRegion('_'));
                    break;
                case "ul":
                    {
                        break;// return BuildList(builder, htmlNode);
                    }
                case "div":
                    {
                        break;// return builder.AddSpoiler(default);
                    }
                case "blockquote":
                    break;
                case "pre":
                    {
                        break;// return builder;
                    }
                case "a" or "img" or "em" or "strong" or "span" or "code":
                    regions.Add(new ParagraphRegion([GetInline(htmlNode)]));
                    break;
            }
        }

        private Models.Inlines.IInline GetInline(HtmlNode node)
        {
            var name = node.Name;

            switch (name) 
            {
                case "p":
                    break;
                case "a":
                    return GetInlineLink(node);
                    break;
                case "img":
                    return GetInlineImage(node);
                case "em":
                    break;
                case "strong":
                    break;
                case "span":
                    break;
                case "code":
                    break;
            }

            return null;
        }

        private Models.Inlines.IInline GetInlineLink(HtmlNode node)
        {
            //if (node.FirstChild)

            return null;
            //if (node.)
        }

        private Models.Inlines.IInline GetInlineImage(HtmlNode htmlNode)
        {
            var src = string.Empty;
            var alt = string.Empty;
            var width = 0.0;
            var height = 0.0;

            foreach (var attr in htmlNode.Attributes)
            {
                if (attr.Name == "src")
                {
                    src = attr.Value;
                }

                if (attr.Name == "alt")
                {
                    alt = attr.Value;
                }

                if (attr.Name == "width")
                {
                    width = double.Parse(attr.Value);
                }

                if (attr.Name == "height")
                {
                    height = double.Parse(attr.Value);
                }
            }

            return new InlineImage(alt, src, width: width, height: height);
        }

        private IRegion ToHeaderRegion(HtmlNode htmlNode)
        {
            return new HeadingRegion([new Paragraph(htmlNode.InnerText)], int.Parse(htmlNode.Name.Substring(1)));
                // headingBlock.Level);
                //inlines
        }
    }
}
