using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AppStudio.Uwp.Navigation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;

namespace Windows10News.Navigation
{
    public class AppNavigation
    {
        private NavigationNode _active;

        static AppNavigation()
        {

        }

        public NavigationNode Active
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active != null)
                {
                    _active.IsSelected = false;
                }
                _active = value;
                if (_active != null)
                {
                    _active.IsSelected = true;
                }
            }
        }


        public ObservableCollection<NavigationNode> Nodes { get; private set; }

        public void LoadNavigation()
        {
            Nodes = new ObservableCollection<NavigationNode>();
		    var resourceLoader = new ResourceLoader();
            Nodes.Add(new ItemNavigationNode
            {
                Title = @"Windows 10 News",
                Label = "Home",
                FontIcon = "\ue10f",
                IsSelected = true,
                NavigationInfo = NavigationInfo.FromPage("HomePage")
            });

            Nodes.Add(new ItemNavigationNode
            {
                Label = "What's going on",
                FontIcon = "\ue12a",
                NavigationInfo = NavigationInfo.FromPage("WhatsGoingOnListPage")
            });
            Nodes.Add(new ItemNavigationNode
            {
                Label = "Recent news",
                FontIcon = "\ue1d7",
                NavigationInfo = NavigationInfo.FromPage("RecentNewsListPage")
            });
            Nodes.Add(new ItemNavigationNode
            {
                Label = "Apps",
                FontIcon = "\ue15a",
                NavigationInfo = NavigationInfo.FromPage("AppsListPage")
            });
            Nodes.Add(new ItemNavigationNode
            {
                Label = "Insider program",
                FontIcon = "\ue178",
                NavigationInfo = NavigationInfo.FromPage("InsiderProgramListPage")
            });
            Nodes.Add(new ItemNavigationNode
            {
                Label = "What are people talking about",
                FontIcon = "\ue134",
                NavigationInfo = NavigationInfo.FromPage("WhatArePeopleTalkingAboutListPage")
            });
            Nodes.Add(new ItemNavigationNode
            {
                Label = "Do more",
                FontIcon = "\ue12d",
                NavigationInfo = NavigationInfo.FromPage("DoMoreListPage")
            });
            Nodes.Add(new ItemNavigationNode
            {
                Label = resourceLoader.GetString("NavigationPanePrivacy"),
                FontIcon = "\ue1f7",
                NavigationInfo = new NavigationInfo()
                {
                    NavigationType = NavigationType.DeepLink,
                    TargetUri = new Uri("http://appstudio.windows.com/home/appprivacyterms", UriKind.Absolute)
                }
            });
        }

        public NavigationNode FindPage(Type pageType)
        {
            return GetAllItemNodes(Nodes).FirstOrDefault(n => n.NavigationInfo.NavigationType == NavigationType.Page && n.NavigationInfo.TargetPage == pageType.Name);
        }

        private IEnumerable<ItemNavigationNode> GetAllItemNodes(IEnumerable<NavigationNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is ItemNavigationNode)
                {
                    yield return node as ItemNavigationNode;
                }
                else if(node is GroupNavigationNode)
                {
                    var gNode = node as GroupNavigationNode;

                    foreach (var innerNode in GetAllItemNodes(gNode.Nodes))
                    {
                        yield return innerNode;
                    }
                }
            }
        }
    }
}
