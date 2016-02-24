using System;
using System.Collections.ObjectModel;
using AppStudio.Uwp;
using AppStudio.Uwp.Navigation;
using Windows.UI.Xaml;

namespace Windows10News.Navigation
{
    public abstract class NavigationNode : ObservableBase
    {
        private bool _isSelected;
        public string Title { get; set; }
        public string Label { get; set; }
        public string FontIcon { get; set; }
        public string Image { get; set; }
        public bool IsVisible { get; set; }
        public abstract bool IsContainer { get; }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        public abstract void Selected();

        public override string ToString()
        {
            return Label;
        }
    }

    public class ItemNavigationNode : NavigationNode, INavigable
    {
        public override bool IsContainer
        {
            get
            {
                return false;
            }
        }

        public NavigationInfo NavigationInfo { get; set; }

        public override void Selected()
        {
            NavigationService.NavigateTo(this);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class GroupNavigationNode : NavigationNode
    {
        private bool _isGroupListOpened;
        private double _backgroundOpacity;

        public GroupNavigationNode(string label, string fontIcon, string image, bool isVisible = true, bool isGroupListOpened = true)
        {
            Nodes = new ObservableCollection<NavigationNode>();
            Label = label;
            FontIcon = fontIcon;
            IsVisible = isVisible;
            IsGroupListOpened = isGroupListOpened;
            Image = image;
        }

        public override bool IsContainer
        {
            get
            {
                return true;
            }
        }

        public ObservableCollection<NavigationNode> Nodes { get; set; }
        
        public bool IsGroupListOpened
        {
            get { return _isGroupListOpened; }
            set
            {
                SetProperty(ref _isGroupListOpened, value);
                if (_isGroupListOpened == true)
                {
                    BackgroundOpacity = 0.2;
                }
                else
                {
                    BackgroundOpacity = 0;
                }
            }
        }

        public double BackgroundOpacity
        {
            get { return _backgroundOpacity; }
            set { SetProperty(ref _backgroundOpacity, value); }
        }

        public override void Selected()
        {
            IsGroupListOpened = !IsGroupListOpened;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
