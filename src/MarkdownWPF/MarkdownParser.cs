using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownWPF.Models;
using MarkdownWPF.Models.Regions;

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
            ICollection<IMarkdownElement> elements = [];

            foreach (var mdItem in document)
            {
                if (mdItem is HeadingBlock headingBlock)
                {
                    elements.Add(GetHeader(headingBlock));
                }
                if (mdItem is ParagraphBlock paragraphBlock)
                {
                    elements.Add(new ParagraphRegion(GetInlines(paragraphBlock.Inline)));
                }
                if (mdItem is CodeBlock codeBlock) 
                {
                    elements.Add(ToCodeRegion(codeBlock));
                }
            }

            return elements;
        }

        /// <summary>
        /// Parse ContainerInline (Markdig) to IEnumerable<IInline> (MarkdownWPF)
        /// </summary>
        /// <param name="containerInline">Inline container</param>
        /// <returns>Collection of inline objects</returns>
        public IEnumerable<Models.IInline> GetInlines(ContainerInline? containerInline)
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
        public Models.IInline GetInline(Inline inline)
        {
            Console.WriteLine(inline.GetType());
            if (inline is LiteralInline literalInline)
            {
                return new Paragraph(literalInline.Content.ToString());
            }

            if (inline is EmphasisInline emphasisInline)
            {
                return TraverseEmphasis(emphasisInline, [], [], text: GetTextFromInline(emphasisInline));
            }

            if (inline is LineBreakInline lineBreakInline)
            {
                return new Paragraph("");
            }

            if (inline is CodeInline codeInline) 
            {
                return new InlineCode(codeInline.Content);
            }

            throw new NotSupportedException($"Unknown inline {inline} argument");
        }


        // x - lenght of string before current element
        // x - start current of substring in general string
        // n - end of substring (normazlied for general string)
        // +y - delimeter char count (start and end)
        // 

        public Emphasis TraverseEmphasis(EmphasisInline emphasisInline,
            List<EmphasisTypography> typographyElements,
            IList<EmphasisDecorations> decorations,
            EmphasisStyle style = EmphasisStyle.Normal,
            EmphasisWeight weight = EmphasisWeight.Normal,
            bool hasHighlight = false,
            string text = "")
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
                return TraverseEmphasis(firstEmphasisChild, typographyElements, decorations, style, weight, hasHighlight, text);
            }

            if (emphasisInline.LastChild is EmphasisInline lastEmphasisChild)
            {
                return TraverseEmphasis(lastEmphasisChild, typographyElements, decorations, style, weight, hasHighlight, text);
            }

            return new Emphasis(text, typographyElements, decorations, weight, style, hasHighlight);
        }


        /// <summary>
        /// Parse HeadingBlock (markdig) to IMarkdownElement (MarkdownWpf)
        /// </summary>
        /// <param name="headingBlock">HeadingBlock</param>
        /// <returns>Heading</returns>
        public IMarkdownElement GetHeader(HeadingBlock headingBlock)
        {
            var inlines = new List<Models.IInline>();

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
                codeRegion.Elements.Add(new Paragraph(line.Slice.ToString()));
            }

            var tmpElements = codeRegion.Elements;
            for (var i = tmpElements.Count - 1; i > 0; i--) 
            {
                if (!string.IsNullOrEmpty(tmpElements[i].Text)) 
                {
                    break;
                }

                codeRegion.Elements.RemoveAt(i);
            }

            return codeRegion;
        }
    }
}
