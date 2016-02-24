using AppStudio.Uwp;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Converters;
using AppStudio.Uwp.DataSync;
using AppStudio.Uwp.Navigation;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Media.Imaging;

namespace Windows10News.ViewModels
{
    public class ItemViewModel : ObservableBase, INavigable, ISyncItem<ItemViewModel>
    {
        public string Id { get; set; }
        public object OrderBy { get; set; }
        public object GroupBy { get; set; }

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _subTitle;
        public string SubTitle
        {
            get { return _subTitle; }
            set { SetProperty(ref _subTitle, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { SetProperty(ref _imageUrl, value); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private string _footer;
        public string Footer
        {
            get { return _footer; }
            set { SetProperty(ref _footer, value); }
        }

        private string _aside;
        public string Aside
        {
            get { return _aside; }
            set { SetProperty(ref _aside, value); }
        }

        private string _source;
        public string Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        private IEnumerable<string> SearchFields
        {
            get
            {
                yield return Title;
                yield return SubTitle;
                yield return Description;
                yield return Content;
            }
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
            return this.Id == other.Id && (this.PageTitle != other.PageTitle || this.Title != other.Title || this.SubTitle != other.SubTitle || this.Description != other.Description || this.ImageUrl != other.ImageUrl || this.Content != other.Content);
        }

        public void Sync(ItemViewModel other)
        {
            this.PageTitle = other.PageTitle;
            this.Title = other.Title;
            this.SubTitle = other.SubTitle;
            this.Description = other.Description;
            this.ImageUrl = other.ImageUrl;
            this.Content = other.Content;
            this.Aside = other.Aside;
            this.GroupBy = other.GroupBy;
            this.Source = other.Source;
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

        public bool ContainsString(string stringText)
        {
            if (!string.IsNullOrEmpty(stringText))
            {
                foreach (string searchField in SearchFields)
                {
                    if (!string.IsNullOrEmpty(searchField) && searchField.ToLowerInvariant().Contains(stringText.ToLowerInvariant()))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string LoadSafeUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return StringToSizeConverter.DefaultEmpty;
            }
            try
            {
                if (!imageUrl.StartsWith("http") && !imageUrl.StartsWith("ms-appx:"))
                {
                    imageUrl = string.Concat("ms-appx://", imageUrl);
                }
            }
            catch (Exception) { }
            return imageUrl;
        }
    }
}
