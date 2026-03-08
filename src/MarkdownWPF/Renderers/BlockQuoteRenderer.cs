using Markdig.Renderers;
using Markdig.Syntax;
using System.Windows.Controls;

namespace MarkdownWPF.Renderers
{
    public class BlockQuoteRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, QuoteBlock>
    {
        protected override void Write(WpfVirtualizingRenderer renderer, QuoteBlock quoteBlock)
        {
            // 1. Создаем визуальную обертку (ту самую рамку с левой полосой)
            var border = new Border();
            renderer.ApplyStyle(border, MarkdownStyles.BlockQuote);
            renderer.HandleLastChildMargin(border, quoteBlock);

            // 2. Создаем контейнер для внутреннего содержимого цитаты
            var stackPanel = new StackPanel();
            border.Child = stackPanel;

            // 3. Добавляем цитату в текущее визуальное дерево
            if (renderer.CurrentContext == null)
            {
                renderer.RootElements.Add(border);
            }
            else if (renderer.CurrentContext is Panel parentPanel)
            {
                parentPanel.Children.Add(border);
            }

            // 4. Погружаемся внутрь цитаты
            // Мы пушим stackPanel, чтобы все параграфы и списки внутри цитаты
            // добавлялись именно в эту панель, а не в основной документ.
            renderer.Push(stackPanel);

            renderer.WriteChildren(quoteBlock);

            renderer.Pop();
        }
    }
}
