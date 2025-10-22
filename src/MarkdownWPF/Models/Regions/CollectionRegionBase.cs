using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MarkdownWPF.Models.Regions
{
    public abstract class CollectionRegionBase<T> : RegionBase<IList<T>>
        where T : IMarkdownElement
    {
        protected CollectionRegionBase()
        {
            Value = new List<T>();
        }
    }

    public abstract class RegionBase<T> : IRegion
    {
        public T Value { get; protected set; }
    }

    public abstract class ObservableRegionBase<T> : RegionBase<T>, INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public interface IRegion : IMarkdownElement 
    {

    }
}
