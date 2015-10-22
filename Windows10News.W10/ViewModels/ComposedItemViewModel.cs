using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Windows10News.ViewModels
{
    public class ComposedItemViewModel : ItemViewModel, IEnumerable<ItemViewModel>
    {
        private ObservableCollection<ItemViewModel> _blocks;

        public ComposedItemViewModel()
        {
            _blocks = new ObservableCollection<ItemViewModel>();
        }

        public void Add(ItemViewModel item)
        {
            _blocks.Add(item);

            if (_blocks.Count == 1)
            {
                Sync(item);
            }
        }

        public void Remove(ItemViewModel item)
        {
            _blocks.Remove(item);

            if (_blocks.Count == 0)
            {
                Sync(new ItemViewModel());
            }
        }

        public ItemViewModel this[int index]
        {
            get
            {
                return _blocks[index];
            }
        }

        public IEnumerator<ItemViewModel> GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
