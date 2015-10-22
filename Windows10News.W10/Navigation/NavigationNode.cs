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
        private Visibility _visibility;
        private double _backgroundOpacity;

        public GroupNavigationNode()
        {
            Nodes = new ObservableCollection<NavigationNode>();
        }

        public override bool IsContainer
        {
            get
            {
                return true;
            }
        }

        public ObservableCollection<NavigationNode> Nodes { get; set; }

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                SetProperty(ref _visibility, value);
                if (_visibility == Visibility.Visible)
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
            if (Visibility == Visibility.Collapsed)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
