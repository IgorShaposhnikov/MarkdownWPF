# MarkdownWPF

A high-performance Markdown renderer for WPF built on top of [Markdig](https://github.com/xoofx/markdig).

Instead of rendering everything into a heavy `FlowDocument`, **MarkdownWPF** generates lightweight WPF UI blocks (`TextBlock`, `Border`, `Grid`, etc.) and displays them via an `ItemsControl` with a `VirtualizingStackPanel`. This keeps scrolling smooth and memory usage reasonable even for large Markdown documents.

## Features

- **True UI virtualization**  
  Uses WPF virtualization (`VirtualizingStackPanel` + Recycling) so only visible blocks are realized in the visual tree.

- **Markdig-based parsing**  
  Supports common Markdown and many GitHub-flavored extensions (depending on your Markdig pipeline).

- **XAML-first styling & theming**  
  No hardcoded colors required. Override styles globally (App resources) or locally (UserControl/Window resources).

- **Base-style override pattern**  
  Default styles are split into `*Base` + “working” styles, making it easy to inherit and override without circular `BasedOn` issues.

- **Nested layout friendly spacing**  
  Optional “last-child margin” handling via an attached property + XAML triggers to avoid excessive gaps inside lists/quotes/tables.

- **Image handling (layout + memory)**  
  Images are constrained to the viewer width (`MaxWidth` binding) to prevent horizontal overflow, and can be decoded with a configurable `DecodePixelWidth` to avoid huge RAM spikes when viewing large photos.

## Installation

Install the package:

```bash
dotnet add package MarkdownWPF
```

Or via Visual Studio: **Manage NuGet Packages** → enable **Include prerelease** → search for `MarkdownWPF`.

## Quick start

1) Merge the default theme dictionary:
```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/MarkdownWPF;component/Themes/MarkdownDictionary.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

2) Use the control:
```xml
<Window
    xmlns:md="clr-namespace:MarkdownWPF;assembly=MarkdownWPF">

    <md:MarkdownViewer Markdown="{Binding MarkdownText}" />
</Window>
```


## Customizing styles

All visuals are driven by standard WPF `Style` resources. You can override them:

- Globally in `App.xaml`, or
- Locally inside a `UserControl.Resources` / `Window.Resources`

The library is designed around a safe inheritance model:

- `MarkdownParagraphStyleBase` (base style)
- `MarkdownParagraphStyle` (working style used by the renderer)

To override a working style while keeping defaults:

```xml
<Window.Resources>
    <Style x:Key="MarkdownParagraphStyle"
           TargetType="TextBlock"
           BasedOn="{StaticResource MarkdownParagraphStyleBase}">
        <Setter Property="FontSize" Value="16"/>
        <Setter Property="Foreground" Value="DarkSlateGray"/>
    </Style>
</Window.Resources>
```


## Notes / limitations

- **SVG images**: WPF does not render SVG out of the box. If your Markdown contains SVG badges (common in GitHub READMEs), you will need an SVG rendering library or a fallback strategy.
- This is a **preview** release: APIs, style keys, and render behavior may change.
