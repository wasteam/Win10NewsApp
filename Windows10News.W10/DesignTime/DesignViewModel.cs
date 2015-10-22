using System.Collections.ObjectModel;

namespace Windows10News.ViewModels
{
    public class DesignViewModel
    {
        public ItemViewModel SelectedItem { get; set; }
        public ObservableCollection<ItemViewModel> Items { get; set; }
        public string Title { get; set; }
    }
    public class TouchDevelopDesignViewModel
    {
        public ObservableCollection<TouchDevelopItemDesignViewModel> Items { get; set; }
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
    public class TouchDevelopItemDesignViewModel
    {
        public string IconUrl { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string CumulativePositiveReviews { get; set; }
        public bool HasScreenshot { get; set; }
        public string ScreenshotUrl { get; set; }
        public string Description { get; set; }
    }
}
