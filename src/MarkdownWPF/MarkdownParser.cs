using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownWPF.Models;
using MarkdownWPF.Models.Inlines;
using MarkdownWPF.Models.Regions;
using System.Windows;

namespace MarkdownWPF
{
    public class MarkdownParser
    {
        public IEnumerable<IMarkdownElement> Parse(string text, Func<MarkdownPipeline>? getPipeline = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return [];
            }

            var pipeline = getPipeline == null ?
                new MarkdownPipelineBuilder().UseAdvancedExtensions().Build() : getPipeline();
            var document = Markdown.Parse(text, pipeline);

            return ParseMarkdigDocument(document);
        }

        /// <summary>
        /// Parse MarkdownDocument (Markdig) to IMarkdownElements (MarkdownWPF) objects collection.
        /// </summary>
        /// <param name="document">MarkdownDocument</param>
        /// <returns>Collection of IMarkdownElements</returns>
        private IEnumerable<IMarkdownElement> ParseMarkdigDocument(MarkdownDocument document)
        {
            Console.Clear();
            IList<IRegion> elements = [];

            foreach (var mdItem in document)
            {
                GetRegionByType(mdItem, elements);
            }

            return elements;
        }

        private void GetRegionByType(Block mdItem, IList<IRegion> elements)
        {
            if (mdItem is HeadingBlock headingBlock)
            {
                elements.Add(GetHeader(headingBlock));
            }
            else if (mdItem is ParagraphBlock paragraphBlock)
            {
                var inlinesResult = GetInlines(paragraphBlock.Inline);

                IList<Models.Inlines.IInline> inlines = [];
                foreach (var inline in inlinesResult)
                {
                    if (inline is InlineLink inlineLink && inlineLink.IsImage)
                    {
                        if (inlines.Count > 0)
                        {
                            elements.Add(new ParagraphRegion(inlines));
                            inlines.Clear();
                        }

                        elements.Add(new ImageRegion(inlineLink));

                        continue;
                    }

                    inlines.Add(inline);
                }

                if (inlines.Count > 0)
                {
                    elements.Add(new ParagraphRegion(inlines));
                }
            }
            else if (mdItem is CodeBlock codeBlock)
            {
                elements.Add(ToCodeRegion(codeBlock));
            }
            else if (mdItem is ThematicBreakBlock thematicBreakBlock)
            {
                elements.Add(new ThematicBreakRegion(thematicBreakBlock.ThematicChar, thematicBreakBlock.ThematicCharCount));
            }
            else if (mdItem is QuoteBlock quoteBlock)
            {
                elements.Add(GetQuoteRegion(quoteBlock));
            }
            else if (mdItem is ListBlock listBlock)
            {
                elements.Add(ToListRegion(listBlock));
            }
            else if (mdItem is ListItemBlock listItemBlock)
            {
                elements.Add(ToListItemRegion(listItemBlock));
            }
            else if (mdItem is Table table) 
            {
                elements.Add(ToTableRegion(table));
            }
        }

        /// <summary>
        /// Parse ContainerInline (Markdig) to IEnumerable<IInline> (MarkdownWPF)
        /// </summary>
        /// <param name="containerInline">Inline container</param>
        /// <returns>Collection of inline objects</returns>
        public IEnumerable<Models.Inlines.IInline> GetInlines(ContainerInline? containerInline)
        {
            if (containerInline == null)
            {
                return [];
            }

            return containerInline.Select(GetInline);
        }

        /// <summary>
        /// Parse Inline (Markdig) to Inline (MarkdownWPF)
        /// </summary>
        /// <param name="inline">Markdig Inline</param>
        /// <returns>MarkdownWPF inline type</returns>
        /// <exception cref="NotSupportedException">Unsupported inline type</exception>
        public Models.Inlines.IInline GetInline(Inline inline)
        {
            Console.WriteLine(inline.GetType());
            if (inline is LiteralInline literalInline)
            {
                return new Paragraph(literalInline.Content.ToString());
            }

            if (inline is EmphasisInline emphasisInline)
            {
                return TraverseEmphasis(emphasisInline, new HashSet<EmphasisTypography>(), new HashSet<EmphasisDecorations>(), text: GetTextFromInline(emphasisInline));
            }

            if (inline is LineBreakInline lineBreakInline)
            {
                return new Paragraph("\n");
            }

            if (inline is CodeInline codeInline)
            {
                return new InlineCode(codeInline.Content);
            }

            if (inline is LinkInline linkInline)
            {
                // TODO: URL must be file path
                return new InlineLink(GetTextFromInline(linkInline.FirstChild), linkInline.Url, linkInline.IsImage);
            }

            if (inline is LinkDelimiterInline linkDelimiterInline)
            {
                Console.WriteLine(inline);
                return new Paragraph("");
            }

            throw new NotSupportedException($"Unknown inline {inline.GetType()} argument");
        }


        // x - lenght of string before current element
        // x - start current of substring in general string
        // n - end of substring (normazlied for general string)
        // +y - delimeter char count (start and end)
        // 

        public Emphasis TraverseEmphasis(EmphasisInline emphasisInline,
            ISet<EmphasisTypography> typographyElements,
            ISet<EmphasisDecorations> decorations,
            EmphasisStyle style = EmphasisStyle.Normal,
            EmphasisWeight weight = EmphasisWeight.Normal,
            bool hasHighlight = false,
            string text = "",
            IList<Models.Inlines.IInline> inlines = null)
        {
            if (emphasisInline.DelimiterChar == '*' || emphasisInline.DelimiterChar == '_')
            {
                if (emphasisInline.DelimiterCount == 1)
                {
                    style = EmphasisStyle.Italic;
                }
                else if (emphasisInline.DelimiterCount == 2)
                {
                    weight = EmphasisWeight.Bold;
                }
            }
            else if (emphasisInline.DelimiterChar == '~')
            {
                if (emphasisInline.DelimiterCount == 1)
                {
                    // superscript/subscript unsupported
                }
                else if (emphasisInline.DelimiterCount == 2)
                {
                    decorations.Add(EmphasisDecorations.Strikethrough);
                }
            }
            else if (emphasisInline.DelimiterChar == '+')
            {
                decorations.Add(EmphasisDecorations.Underline);
            }
            else if (emphasisInline.DelimiterChar == '=')
            {
                hasHighlight = true;
            }


            if (emphasisInline.FirstChild is EmphasisInline firstEmphasisChild)
            {
                return TraverseEmphasis(firstEmphasisChild, typographyElements, decorations, style, weight, hasHighlight, text, inlines);
            }

            if (emphasisInline.LastChild is EmphasisInline lastEmphasisChild)
            {
                return TraverseEmphasis(lastEmphasisChild, typographyElements, decorations, style, weight, hasHighlight, text, inlines);
            }

            if (emphasisInline.FirstChild is LinkInline firstLinkInline)
            {
                if (inlines == null)
                {
                    inlines = new List<Models.Inlines.IInline>();
                }

                inlines.Add(new InlineLink(GetTextFromInline(firstLinkInline.FirstChild), firstLinkInline.Url, firstLinkInline.IsImage));
            }

            if (emphasisInline.FirstChild is CodeInline codeInline)
            {
                if (inlines == null)
                {
                    inlines = new List<Models.Inlines.IInline>();
                }

                inlines.Add(new InlineCode(codeInline.Content));
            }

            return new Emphasis(text, typographyElements, decorations, weight, style, hasHighlight, inlines);
        }


        /// <summary>
        /// Parse HeadingBlock (markdig) to IMarkdownElement (MarkdownWpf)
        /// </summary>
        /// <param name="headingBlock">HeadingBlock</param>
        /// <returns>Heading</returns>
        public HeadingRegion GetHeader(HeadingBlock headingBlock)
        {
            var inlines = new List<Models.Inlines.IInline>();

            // select all inline from HeadingBlock
            foreach (var inline in headingBlock.Inline)
            {
                inlines.Add(GetInline(inline));
            }

            return new HeadingRegion(
                inlines,
                headingBlock.Level);
        }


        /// <summary>
        /// Return text from any inline element of Markdig.
        /// </summary>
        /// <typeparam name="T">Any Inline markdig type </typeparam>
        /// <param name="inline">Inline object</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string GetTextFromInline<T>(T inline) where T : Inline
        {
            if (inline == null)
            {
                return string.Empty;
            }

            if (inline is LiteralInline literalInline)
            {
                return literalInline.Content.ToString();
            }

            if (inline is ContainerInline containerInline)
            {
                var text = string.Empty;

                return GetContainerInlineText(containerInline);
            }

            throw new ArgumentException($"Unknown inline {inline} argument");
        }

        public string GetContainerInlineText(ContainerInline containerInline)
        {
            var text = "";
            foreach (var child in containerInline)
            {
                if (child is LiteralInline literalInline)
                {
                    // TODO: Use string builder by size of content?
                    text += (child as LiteralInline).Content.ToString();
                }
                else if (child is ContainerInline container)
                {
                    text += GetContainerInlineText(container);
                }
            }
            return text;
        }

        public CodeRegion ToCodeRegion(CodeBlock codeBlock)
        {
            var codeRegion = new CodeRegion();

            if (codeBlock.Lines.Lines == null)
            {
                return codeRegion;
            }

            foreach (var line in codeBlock.Lines.Lines)
            {
                codeRegion.Value.Add(new Paragraph(line.Slice.ToString()));
            }

            var tmpElements = codeRegion.Value;
            for (var i = tmpElements.Count - 1; i > 0; i--)
            {
                if (!string.IsNullOrEmpty(tmpElements[i].Text))
                {
                    break;
                }

                codeRegion.Value.RemoveAt(i);
            }

            return codeRegion;
        }

        public QuoteRegion GetQuoteRegion(QuoteBlock quoteBlock)
        {
            var region = new QuoteRegion();

            foreach (var i in quoteBlock)
            {
                GetRegionByType(i, region.Value);
            }

            return region;
        }
        public static bool CheckURLValid(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uriResult))
            {
                try
                {
                    return uriResult.Scheme == Uri.UriSchemeHttp
                        || uriResult.Scheme == Uri.UriSchemeHttps
                        || uriResult.Scheme == Uri.UriSchemeFile;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public ListRegion ToListRegion(ListBlock listBlockRoot)
        {
            var listRegion = new ListRegion();

            foreach (ListItemBlock listItemBlock in listBlockRoot)
            {
                var listItemRegions = new List<IRegion>();
                GetRegionByType(listItemBlock, listItemRegions);

                foreach (ListItemRegion i in listItemRegions) 
                {
                    listRegion.Value.Add(i);
                }
            }

            return listRegion;
        }

        private ListItemRegion ToListItemRegion(ListItemBlock listItemBlock)
        {
            var parent = (listItemBlock.Parent as ListBlock);

            var listItemRegion = new ListItemRegion(new List<IRegion>())
            {
                IsOrdered = parent.IsOrdered,
                BulletType = parent.BulletType,
                OrderedStart = parent.OrderedStart,
                OrderedDelimiter = parent.IsOrdered ? parent.OrderedDelimiter : char.MinValue,
                Order = listItemBlock.Order
            };

            foreach (var i in listItemBlock)
            {
                GetRegionByType(i, listItemRegion.Value);
            }

            return listItemRegion;
        }

        private TableRegion ToTableRegion(Table table)
        {
            var tableRegion = new TableRegion();

            foreach (TableRow row in table)
            {
                tableRegion.Value.Add(ToTableRowRegion(row));
            }

            return tableRegion;
        }

        private TableRowRegion ToTableRowRegion(TableRow row)
        {
            var rowRegion = new TableRowRegion();

            foreach (TableCell cell in row)
            {
                rowRegion.Value.Add(ToTableCellRegion(cell));
            }

            return rowRegion;
        }

        private TableCellRegion ToTableCellRegion(TableCell tableCell)
        {
            var cellRegion = new TableCellRegion();

            foreach (var i in tableCell)
            {
                GetRegionByType(i, cellRegion.Value);
            }

            return cellRegion;
        }
    }
}
