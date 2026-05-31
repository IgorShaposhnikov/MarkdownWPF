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
		private static readonly Regex _ellipsisPattern = new(@"(?<![!?])\.{2,}", RegexOptions.Compiled);
		private static readonly Regex _punctuationDotsPattern = new(@"([!?])\.{2,}", RegexOptions.Compiled);
		private static readonly Regex _exclamationPattern = new(@"!{4,}", RegexOptions.Compiled);
		private static readonly Regex _questionPattern = new(@"\?{3,}", RegexOptions.Compiled);
		private static readonly Regex _commaPattern = new(@",{2,}", RegexOptions.Compiled);

		private static string ReplaceTypography(string text)
		{
			text = _punctuationDotsPattern.Replace(text, "$1..");
			text = _ellipsisPattern.Replace(text, "\u2026");
			text = text.Replace("---", "\u2014").Replace("--", "\u2013");
			text = _exclamationPattern.Replace(text, "!!!");
			text = _questionPattern.Replace(text, "???");
			text = _commaPattern.Replace(text, ",");

			return text
				.Replace("(c)", "\u00A9")
				.Replace("(C)", "\u00A9")
				.Replace("(r)", "\u00AE")
				.Replace("(R)", "\u00AE")
				.Replace("(tm)", "\u2122")
				.Replace("(TM)", "\u2122")
				.Replace("(p)", "\u00A7")
				.Replace("(P)", "\u00A7")
				.Replace("+-", "\u00B1");
		}

		protected override void Write(WpfVirtualizingRenderer renderer, LiteralInline obj)
		{
			var text = ReplaceTypography(obj.Content.ToString());

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