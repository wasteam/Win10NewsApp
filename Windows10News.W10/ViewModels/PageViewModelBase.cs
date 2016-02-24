using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using AppStudio.DataProviders;
using AppStudio.DataProviders.Core;
using AppStudio.Uwp;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Cache;
using Windows.ApplicationModel.DataTransfer;
using Windows10News.Config;

namespace Windows10News.ViewModels
{
    public abstract class PageViewModelBase : ObservableBase
    {
        protected readonly TimeSpan CacheExpiration = new TimeSpan(2, 0, 0);

        private string _title;
        private bool _hasLoadDataErrors;
        private bool _isBusy;
        private DateTime? _lastUpdated;

        public string SectionName { get; set; }

        protected PageViewModelBase()
        {
            Actions = new List<ActionInfo>();
        }

        public string Title
        {
            get { return _title; }
            protected set { SetProperty(ref _title, value); }
        }

        public bool HasLoadDataErrors
        {
            get { return _hasLoadDataErrors; }
            protected set { SetProperty(ref _hasLoadDataErrors, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            protected set { SetProperty(ref _isBusy, value); }
        }

        public DateTime? LastUpdated
        {
            get { return _lastUpdated; }
            protected set { SetProperty(ref _lastUpdated, value); }
        }

        public List<ActionInfo> Actions { get; private set; }

        public bool HasActions
        {
            get
            {
                return Actions != null && Actions.Count > 0;
            }
        }

        public bool HasLocalData { get; set; }

        protected void ShareContent(DataRequest dataRequest, ItemViewModel item, bool supportsHtml)
        {
            if (item != null)
            {
                dataRequest.Data.Properties.Title = string.IsNullOrEmpty(item.Title) ? Title : item.Title;

                if (!string.IsNullOrEmpty(item.SubTitle))
                {
                    SetContent(dataRequest, item.SubTitle, false);
                }

                if (!string.IsNullOrEmpty(item.Description))
                {
                    SetContent(dataRequest, item.Description, supportsHtml);
                }

                if (!string.IsNullOrEmpty(item.Content))
                {
                    SetContent(dataRequest, item.Content, supportsHtml);
                }

                if (!string.IsNullOrEmpty(item.Source))
                {
                    dataRequest.Data.SetWebLink(new Uri(item.Source));
                }

                var imageUrl = item.ImageUrl;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(item.Source))
                        {
                            dataRequest.Data.SetWebLink(new Uri(imageUrl));
                        }
                    }
                    else
                    {
                        imageUrl = string.Format("ms-appx://{0}", imageUrl);
                    }
                    dataRequest.Data.SetBitmap(Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri(imageUrl)));
                }
            }
        }

        private void SetContent(DataRequest dataRequest, string data, bool supportsHtml)
        {
            if (supportsHtml)
            {
                dataRequest.Data.SetHtmlFormat(HtmlFormatHelper.CreateHtmlFormat(data));
            }
            else
            {
                dataRequest.Data.SetText(data.DecodeHtml());
            }
        }
    }
}