using System.Collections.Generic;
using AppStudio.Uwp;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.DataSync;
using AppStudio.Uwp.Navigation;
using Windows.ApplicationModel.Resources;

namespace Windows10News.ViewModels
{
    public class ItemViewModel : ObservableBase, INavigable, ISyncItem<ItemViewModel>
    {
        private string _pageTitle;
        private string _title;
        private string _subTitle;
        private string _description;
        private string _image;
        private string _content;

        public string Id { get; set; }

        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set { SetProperty(ref _subTitle, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public string Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        public NavigationInfo NavigationInfo { get; set; }

        public List<ActionInfo> Actions { get; set; }

        public bool HasActions
        {
            get
            {
                return Actions != null && Actions.Count > 0;
            }
        }

        public bool NeedSync(ItemViewModel other)
        {
            return this.Id == other.Id && (this.PageTitle != other.PageTitle || this.Title != other.Title || this.SubTitle != other.SubTitle || this.Description != other.Description || this.Image != other.Image || this.Content != other.Content);
        }

        public void Sync(ItemViewModel other)
        {
            this.PageTitle = other.PageTitle;
            this.Title = other.Title;
            this.SubTitle = other.SubTitle;
            this.Description = other.Description;
            this.Image = other.Image;
            this.Content = other.Content;
        }

        public bool Equals(ItemViewModel other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;

            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ItemViewModel);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            var resourceLoader = new ResourceLoader();
            string toStringResult = string.Empty;
            if (!string.IsNullOrEmpty(Title))
            {
                toStringResult += resourceLoader.GetString("NarrationTitle") + ". ";
                toStringResult += Title + ". ";
            }
            if (!string.IsNullOrEmpty(SubTitle))
            {
                toStringResult += resourceLoader.GetString("NarrationSubTitle") + ". ";
                toStringResult += SubTitle;
            }
            return toStringResult;
        }
    }
}
