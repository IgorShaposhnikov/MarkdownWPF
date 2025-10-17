# MarkdownWPF
MarkdownWPF is an extenision of [Markdig](https://github.com/xoofx/markdig) for [WPF](https://github.com/dotnet/wpf) that focused on adding Markdown rendering with virtualization support.

> NOTE: The repository is under construction. There will be a proper documentation at some point (or I just dream about it :) )!

## Result

<img width="1920" height="1020" alt="image" src="https://github.com/user-attachments/assets/1db0a9d2-e237-4c48-8710-518d4fe06439" />

## Header Template Example
___

```xaml
<DataTemplate DataType="{x:Type regions:HeadingRegion}">
    <TextBlock
        Margin="0,10,0,5"
        extensions:TextBlockExtensions.MarkdownRenderer="{StaticResource MdRenderer}"
        extensions:TextBlockExtensions.Source="{Binding Value}"
        TextWrapping="WrapWithOverflow">

        <TextBlock.Style>
            <Style TargetType="TextBlock">
                <Setter Property="FontSize" Value="40" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding Level}" Value="2">
                        <Setter Property="FontSize" Value="32" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding Level}" Value="3">
                        <Setter Property="FontSize" Value="24" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding Level}" Value="4">
                        <Setter Property="FontSize" Value="16" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding Level}" Value="5">
                        <Setter Property="FontSize" Value="14" />
                    </DataTrigger>
                    <DataTrigger Binding="{Binding Level}" Value="6">
                        <Setter Property="FontSize" Value="12" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </TextBlock.Style>
    </TextBlock>
</DataTemplate>
```
