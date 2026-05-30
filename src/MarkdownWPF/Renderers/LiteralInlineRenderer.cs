using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MarkdownWPF.Renderers
{
	public class LiteralInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, LiteralInline>
	{
		private static readonly Regex _emailPattern = new(@"([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})", RegexOptions.Compiled);

		protected override void Write(WpfVirtualizingRenderer renderer, LiteralInline obj)
		{
			var text = obj.Content.ToString();

			if (!_emailPattern.IsMatch(text))
			{
				AddInline(renderer, new Run(text));
				return;
			}

			var lastIndex = 0;
			foreach (Match match in _emailPattern.Matches(text))
			{
				if (match.Index > lastIndex)
					AddInline(renderer, new Run(text.Substring(lastIndex, match.Index - lastIndex)));

				var link = new Hyperlink();
				link.Inlines.Add(new Run(match.Value));

				if (Uri.TryCreate("mailto:" + match.Value, UriKind.Absolute, out var uri))
				{
					link.NavigateUri = uri;
					link.RequestNavigate += (s, e) =>
					{
						try { Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true }); }
						catch { }
						e.Handled = true;
					};
				}

				renderer.ApplyStyle(link, MarkdownStyles.Link);
				AddInline(renderer, link);

				lastIndex = match.Index + match.Length;
			}

			if (lastIndex < text.Length)
				AddInline(renderer, new Run(text.Substring(lastIndex)));
		}

		private static void AddInline(WpfVirtualizingRenderer renderer, System.Windows.Documents.Inline inline)
		{
			if (renderer.CurrentContext is Span span)
				span.Inlines.Add(inline);
			else if (renderer.CurrentContext is TextBlock tb)
				tb.Inlines.Add(inline);
		}
	}
}