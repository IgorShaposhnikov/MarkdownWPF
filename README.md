# MarkdownWPF

A high-performance, UI-virtualized Markdown renderer for WPF built on [Markdig](https://github.com/xoofx/markdig).

Instead of rendering into a heavy `FlowDocument`, **MarkdownWPF** generates lightweight WPF elements (`TextBlock`, `Border`, `Grid`, etc.) and hosts them in an `ItemsControl` with a `VirtualizingStackPanel` (Recycling mode). Only visible blocks are realized in the visual tree, keeping scrolling smooth and memory usage low even for very large documents.

## Features

- **True UI virtualization** — `VirtualizingStackPanel` + Recycling mode
- **Multi-target** — `net8.0-windows` and `net472`
- **Markdig pipeline** — CommonMark + GFM extensions via `.UseAdvancedExtensions()`
- **XAML styling** — Base + override pattern, Static and Dynamic resource resolution
- **HTML-in-markdown** — Optional extension for `<table>`, `<ul>`/`<ol>`, `<img>`, `<a>`, inline formatting
- **GFM task lists** — Rendered as interactive `CheckBox` elements
- **Image caching** — Static `ConcurrentDictionary` avoids redundant network/disk loads on re-render
- **Nested scrolling** — `IsScrollViewerEnabled` property for embedding in external `ScrollViewer`
- **Error diagnostics** — `ResourceLoadFailed` event for failed image loads and link navigation
- **Image memory management** — Configurable `DecodePixelWidth` prevents huge RAM spikes
- **Last-child margin suppression** — Attached property + XAML triggers avoid excessive gaps inside lists/quotes/tables

## Installation

```bash
dotnet add package MarkdownWPF
```

For HTML-in-markdown support, also add:

```bash
dotnet add package MarkdownWPF.Html
```

## Quick start

### 1. Merge the default theme

Add the built-in resource dictionary to your `App.xaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/MarkdownWPF;component/Themes/MarkdownDefaultTheme.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### 2. Add the control

```xml
<Window xmlns:md="clr-namespace:MarkdownWPF;assembly=MarkdownWPF">

    <md:MarkdownViewer Markdown="{Binding MarkdownText}" />
</Window>
```

That's it. The `MarkdownViewer` uses the default Markdig pipeline (`.UseAdvancedExtensions()`) and renders your Markdown into virtualized blocks.

### 3. (Optional) Enable HTML rendering

If you installed `MarkdownWPF.Html`, register the HTML extension in your pipeline:

```csharp
using Markdig;
using MarkdownWPF.Html;

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseWpfHtml() // registers HTML tag renderers
    .Build();
```

Then pass the pipeline to the viewer:

```xml
<md:MarkdownViewer
    Markdown="{Binding MarkdownText}"
    Pipeline="{Binding Pipeline}" />
```

## Properties

### `Markdown` (string)

The raw Markdown text to render. Changes are applied automatically.

### `Pipeline` (MarkdownPipeline)

The Markdig pipeline to use. Defaults to `.UseAdvancedExtensions()` if not set.

### `StyleResourceMode` (Static | Dynamic)

Controls how WPF styles are resolved:

| Mode      | Behavior |
|-----------|----------|
| `Static`  | Styles are looked up once via `TryFindResource` and cached. Best for performance when styles don't change at runtime. (Default) |
| `Dynamic` | Uses `SetResourceReference` so style changes are picked up live. Useful for theme switching, but has a slight overhead. |

```xml
<md:MarkdownViewer StyleResourceMode="Dynamic" />
```

### `IsScrollViewerEnabled` (bool, default true)

When `true`, the `MarkdownViewer` includes a built-in `ScrollViewer`. Set to `false` when embedding inside an external `ScrollViewer` to avoid nested scroll conflicts:

```xml
<ScrollViewer VerticalScrollBarVisibility="Auto">
    <StackPanel>
        <TextBlock Text="App header" FontSize="24" />
        <md:MarkdownViewer
            Markdown="{Binding MarkdownText}"
            IsScrollViewerEnabled="False" />
    </StackPanel>
</ScrollViewer>
```

### `ImageMaxDecodeWidth` (int, default 0)

Limits the decode size of images. When set to `0`, the viewer automatically uses the screen or window width. This prevents loading a 4000 px photo at full resolution when it will be displayed at 800 px.

### `ResourceLoadFailed` (event)

Fires when an image fails to load or a hyperlink cannot be navigated:

```csharp
viewer.ResourceLoadFailed += (s, e) =>
{
    Console.WriteLine($"Failed: {e.Url}, Error: {e.Exception?.Message}");
};
```

## Virtualization & performance

The `MarkdownViewer` extends `ItemsControl` and uses:

- `VirtualizingStackPanel` as the `ItemsPanel`
- `VirtualizationMode.Recycling` — reuses container elements
- `ScrollUnit.Pixel` — smooth, per-pixel scrolling

This means:

- **Only visible items** are created and measured. Off-screen items are recycled.
- **Scrolling is smooth** even with hundreds of blocks.
- **Memory stays low** — large documents don't keep thousands of UIElements alive.

## Customizing styles

All visuals are driven by standard WPF `Style` resources. The library uses a safe **Base + override** pattern:

| Base style (always available) | Working style (override this)  | Target type |
|-------------------------------|--------------------------------|-------------|
| `MarkdownParagraphStyleBase`  | `MarkdownParagraphStyle`       | TextBlock   |
| `MarkdownHeading1StyleBase`   | `MarkdownHeading1Style`        | TextBlock   |
| `MarkdownHeading2StyleBase`   | `MarkdownHeading2Style`        | TextBlock   |
| `MarkdownHeading3StyleBase`   | `MarkdownHeading3Style`        | TextBlock   |
| `MarkdownHeading4StyleBase`   | `MarkdownHeading4Style`        | TextBlock   |
| `MarkdownHeading5StyleBase`   | `MarkdownHeading5Style`        | TextBlock   |
| `MarkdownHeading6StyleBase`   | `MarkdownHeading6Style`        | TextBlock   |
| `MarkdownCodeBlockBorderStyleBase` | `MarkdownCodeBlockBorderStyle` | Border  |
| `MarkdownCodeBlockStyleBase`  | `MarkdownCodeBlockStyle`       | TextBlock   |
| `MarkdownThematicBreakStyleBase` | `MarkdownThematicBreakStyle` | Border    |
| `MarkdownListStyleBase`       | `MarkdownListStyle`            | StackPanel  |
| `MarkdownListItemStyleBase`   | `MarkdownListItemStyle`        | Grid        |
| `MarkdownListItemMarkerStyleBase` | `MarkdownListItemMarkerStyle` | TextBlock |
| `MarkdownTableStyleBase`      | `MarkdownTableStyle`           | Grid        |
| `MarkdownTableCellStyleBase`  | `MarkdownTableCellStyle`       | Border      |
| `MarkdownTableHeaderCellStyleBase` | `MarkdownTableHeaderCellStyle` | Border  |
| `MarkdownBlockquoteStyleBase` | `MarkdownBlockQuoteStyle`      | Border      |
| (none)                        | `MarkdownLinkStyle`            | Hyperlink   |
| (none)                        | `MarkdownCodeInlineStyle`      | Run         |
| (none)                        | `MarkdownStrongStyle`          | Span        |
| (none)                        | `MarkdownEmphasisStyle`        | Span        |
| (none)                        | `MarkdownStrikethroughStyle`   | Span        |
| (none)                        | `MarkdownMarkStyle`            | Span        |

If a working style is not found in the resource tree, the renderer **automatically falls back** to the matching Base style — so partial overrides work without errors.

### Example: Make H1 headings red

```xml
<Window.Resources>
    <Style x:Key="MarkdownHeading1Style"
           TargetType="TextBlock"
           BasedOn="{StaticResource MarkdownHeading1StyleBase}">
        <Setter Property="Foreground" Value="Red" />
    </Style>
</Window.Resources>
```

Heading levels 2–6 continue to use their default styles — no extra work needed.

### Example: Override link color

```xml
<Style x:Key="MarkdownLinkStyle" TargetType="Hyperlink">
    <Setter Property="Foreground" Value="Green" />
</Style>
```

## Image caching

Images are cached in a static `ConcurrentDictionary<string, BitmapImage>` by URL. When the Markdown content changes and trigger a full re-render (e.g. while typing in an editor), previously loaded images are served from cache — no redundant HTTP requests or disk reads.

Use `ImageCache.Clear()` to flush the cache if needed.

## HTML extension reference

When using `MarkdownWPF.Html` with `.UseWpfHtml()`, the following HTML tags are supported inside Markdown:

| Tag | Rendered as |
|-----|-------------|
| `<table>`, `<tr>`, `<td>`, `<th>` | Grid + Border cells |
| `<ul>`, `<ol>`, `<li>` | StackPanel + Grid with markers |
| `<img>` | Image (cached) |
| `<a>` | Hyperlink |
| `<b>`, `<strong>` | Bold |
| `<i>`, `<em>` | Italic |
| `<u>`, `<ins>` | Underline |
| `<s>`, `<strike>`, `<del>` | Strikethrough |
| `<code>` | Inline code span |
| `<br>` | Line break |

## Notes / limitations

- **SVG images**: WPF does not render SVG out of the box. If your Markdown contains SVG badges, you will need an SVG rendering library or a fallback strategy.
- **`BitmapCacheOption.OnLoad`**: Network images are loaded synchronously during rendering. For very slow servers this may briefly block the UI thread.
