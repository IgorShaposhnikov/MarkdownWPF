using MarkdownWPF.Models.Inlines;

namespace MarkdownWPF.Models.Regions
{
    public class ImageRegion : ObservableRegionBase<InlineLink>
    {
        public string Title { get; }

        private double _width;

        public double Width
        {
            get => _width; set
            {
                _width = value;
                OnPropertyChanged();
            }
        }

        private double _height;
        public double Height
        {
            get => _height; set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        public ImageRegion(InlineImage image)
        {
            Value = image;
            Title = image.Text;
            Width = image.Width;
            Height = image.Height;
        }
    }
}
