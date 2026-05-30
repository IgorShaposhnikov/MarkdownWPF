using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MarkdownWPF.Renderers
{
	public class AutolinkInlineRenderer : MarkdownObjectRenderer<WpfVirtualizingRenderer, AutolinkInline>
	{
		protected override void Write(WpfVirtualizingRenderer renderer, AutolinkInline obj)
		{
			var url = obj.Url;
			if (obj.IsEmail && !url.StartsWith("mailto:"))
				url = "mailto:" + url;

			var link = new Hyperlink();
			link.Inlines.Add(new Run(obj.Url));

			if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var linkUri))
			{
				link.NavigateUri = linkUri;
				link.RequestNavigate += (sender, e) =>
				{
					try
					{
						if (e.Uri.Scheme == Uri.UriSchemeHttp || e.Uri.Scheme == Uri.UriSchemeHttps || e.Uri.Scheme == Uri.UriSchemeMailto)
							Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
					}
					catch { }
					e.Handled = true;
				};
			}

			renderer.ApplyStyle(link, MarkdownStyles.Link);

			if (renderer.CurrentContext is Span span)
				span.Inlines.Add(link);
			else if (renderer.CurrentContext is TextBlock tb)
				tb.Inlines.Add(link);
		}
	}
}
