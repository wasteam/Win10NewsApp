//---------------------------------------------------------------------------
//
// <copyright file="WhatArePeopleTalkingAboutListPage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>10/22/2015 10:25:19 AM</createdOn>
//
//---------------------------------------------------------------------------

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AppStudio.DataProviders.Twitter;
using Windows10News.Sections;
using Windows10News.ViewModels;

namespace Windows10News.Views
{
    public sealed partial class WhatArePeopleTalkingAboutListPage : Page
    {
        public WhatArePeopleTalkingAboutListPage()
        {
            this.ViewModel = new ListViewModel<TwitterDataConfig, TwitterSchema>(new WhatArePeopleTalkingAboutConfig());
            this.InitializeComponent();
            new Microsoft.ApplicationInsights.TelemetryClient().TrackPageView(this.GetType().FullName);
        }

        public ListViewModel<TwitterDataConfig, TwitterSchema> ViewModel { get; set; }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ViewModel.LoadDataAsync();

            base.OnNavigatedTo(e);
        }

    }
}
