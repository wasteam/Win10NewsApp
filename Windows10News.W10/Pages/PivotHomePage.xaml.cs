//---------------------------------------------------------------------------
//
// <copyright file="PivotHomePage.xaml.cs" company="Microsoft">
//    Copyright (C) 2015 by Microsoft Corporation.  All rights reserved.
// </copyright>
//
// <createdOn>2/24/2016 2:21:07 PM</createdOn>
//
//---------------------------------------------------------------------------

using AppStudio.Uwp;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows10News.ViewModels;

namespace Windows10News.Pages
{
    public sealed partial class PivotHomePage : Page
    {
        public PivotHomePage()
        {
            ViewModel = new MainViewModel(0);            			
            AboutThisAppModel = new AboutThisAppViewModel();
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            new Microsoft.ApplicationInsights.TelemetryClient().TrackPageView(this.GetType().FullName);
        }

        public MainViewModel ViewModel { get; set; }
        public AboutThisAppViewModel AboutThisAppModel { get; private set; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ViewModel.LoadDataAsync();
            searchCtrl.Reset();
			searchCtrl.IsTextVisibleChanged += SearchCtrlIsTextVisibleChanged;
        }
		protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            searchCtrl.IsTextVisibleChanged -= SearchCtrlIsTextVisibleChanged;
        }
		
        private double appBarWidth;
        private void SearchCtrlIsTextVisibleChanged(object sender, bool isTextVisible)
        {
            if (appBar.ActualWidth > 0)
            {
                appBarWidth = appBar.ActualWidth;
            }
            if (isTextVisible)
            {
                appBar.AnimateWidth(0);
            }
            else
            {
                appBar.AnimateWidth(appBarWidth);
            }
        }
		private void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;
            if (pivot != null && pivot.SelectedIndex >= 0 && pivot.Items != null && pivot.Items.Count > 0)
            {
                PivotItem pivotItem = pivot.Items[pivot.SelectedIndex] as PivotItem;                                
                new Microsoft.ApplicationInsights.TelemetryClient().TrackEvent(pivotItem.Header.ToString());
            }
        }
    }
}
