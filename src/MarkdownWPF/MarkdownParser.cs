using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownWPF.Models;

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
            ICollection<IMarkdownElement> elements = [];

            foreach (var mdItem in document) 
            {
                if (mdItem is HeadingBlock headingBlock)
                {
                    elements.Add(GetHeader(headingBlock));
                }
                if (mdItem is ParagraphBlock paragraphBlock)
                {
                    foreach (var inline in GetInlines(paragraphBlock.Inline))
                    {
                        elements.Add(inline);
                    }
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
            if (inline is LiteralInline literalInline)
            {
                Console.WriteLine(literalInline.Content.ToString());
                return new Paragraph(literalInline.Content.ToString());
            }

            if (inline is EmphasisInline emphasisInline)
            {
                EmphasisStyle style = EmphasisStyle.Normal;
                EmphasisDecorations decorations = EmphasisDecorations.None;
                EmphasisWeight weight = EmphasisWeight.Normal;

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

                return new Emphasis(GetTextFromInline(emphasisInline), weight, decorations, style);
            }

            throw new NotSupportedException($"Unknown inline {inline} argument");
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

            return new Heading(
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
                Console.WriteLine(literalInline.Content.ToString());
                return literalInline.Content.ToString();
            }

            if (inline is ContainerInline containerInline)
            {
                var text = string.Empty;

                foreach (var child in containerInline)
                {
                    if (child is LiteralInline)
                    {
                        literalInline = child as LiteralInline;
                        // TODO: Use string builder by size of content?
                        text += literalInline.Content.ToString();
                        Console.WriteLine(text);
                    }
                }

                return text;
            }

            throw new ArgumentException($"Unknown inline {inline} argument");
        }
    }
}
