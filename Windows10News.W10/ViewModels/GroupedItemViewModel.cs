using AppStudio.Uwp;
using AppStudio.Uwp.DataSync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Windows10News.ViewModels
{
    public class GroupedItemViewModel : ObservableBase, ISyncItem<GroupedItemViewModel>
    {
        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }
        private ObservableCollection<ItemViewModel> _items;
        public ObservableCollection<ItemViewModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public void Sync(GroupedItemViewModel other)
        {
            Items.Sync(other.Items);
        }

        public bool NeedSync(GroupedItemViewModel other)
        {
            //always try to sync Items
            return true;
        }

        public bool Equals(GroupedItemViewModel other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return this.Header == other.Header;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GroupedItemViewModel);
        }

        public override int GetHashCode()
        {
            return this.Header.GetHashCode();
        }
    }
}