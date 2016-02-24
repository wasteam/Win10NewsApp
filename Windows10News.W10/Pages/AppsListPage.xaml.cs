//---------------------------------------------------------------------------
//
// <copyright file="AppsListPage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>2/24/2016 2:21:07 PM</createdOn>
//
//---------------------------------------------------------------------------

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using AppStudio.DataProviders.Rss;
using Windows10News.Sections;
using Windows10News.ViewModels;
using AppStudio.Uwp;

namespace Windows10News.Pages
{
    public sealed partial class AppsListPage : Page
    {
	    public ListViewModel ViewModel { get; set; }
        public AppsListPage()
        {
			this.ViewModel = ListViewModel.CreateNew(Singleton<AppsConfig>.Instance);

            this.InitializeComponent();

            new Microsoft.ApplicationInsights.TelemetryClient().TrackPageView(this.GetType().FullName);
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ViewModel.LoadDataAsync();
            base.OnNavigatedTo(e);
        }

    }
}
