using Markdig.Extensions.SmartyPants;
using Markdig.Renderers;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MarkdownWPF.Renderers
{
	public class SmartyPantRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, SmartyPant>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, SmartyPant obj)
		{
			var text = obj.Type switch
			{
				SmartyPantType.Quote => "\u2018",
				SmartyPantType.LeftQuote => "\u2018",
				SmartyPantType.RightQuote => "\u2019",
				SmartyPantType.DoubleQuote => "\u201C",
				SmartyPantType.LeftDoubleQuote => "\u201C",
				SmartyPantType.RightDoubleQuote => "\u201D",
				SmartyPantType.LeftAngleQuote => "\u00AB",
				SmartyPantType.RightAngleQuote => "\u00BB",
				SmartyPantType.Ellipsis => "\u2026",
				SmartyPantType.Dash2 => "\u2013",
				SmartyPantType.Dash3 => "\u2014",
				_ => ""
			};

			if (string.IsNullOrEmpty(text))
				return;

			var run = new Run(text);

			if (renderer.CurrentContext is Span span)
				span.Inlines.Add(run);
			else if (renderer.CurrentContext is TextBlock tb)
				tb.Inlines.Add(run);
		}
	}
}
