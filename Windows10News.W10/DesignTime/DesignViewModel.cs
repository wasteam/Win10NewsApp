using System.Collections.ObjectModel;

namespace Windows10News.ViewModels
{
    public class DesignViewModel
    {
        public ItemViewModel SelectedItem { get; set; }
        public ObservableCollection<ItemViewModel> Items { get; set; }
        public string Title { get; set; }
    }
    public class DetailDesignViewModel
    {
        public DesignViewModel ViewModel { get; set; }
    }
    public class ItemDesignViewModel
    {
        public string Id { get; set; }
        public string PageTitle { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
    }
}
