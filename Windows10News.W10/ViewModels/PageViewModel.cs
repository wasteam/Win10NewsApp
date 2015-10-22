using AppStudio.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Windows10News.ViewModels
{
    public abstract class PageViewModel : ObservableBase
    {        
        private Thickness _pageTitleMargin;
        private int _appBarRow;
        private int _appBarColumn;
        private int _appBarColumnSpan;
        public Thickness PageTitleMargin
        {
            get { return _pageTitleMargin; }
            set { SetProperty(ref _pageTitleMargin, value); }
        }
        public int AppBarRow
        {
            get { return _appBarRow; }
            set { SetProperty(ref _appBarRow, value); }
        }
        public int AppBarColumn
        {
            get { return _appBarColumn; }
            set { SetProperty(ref _appBarColumn, value); }
        }
        public int AppBarColumnSpan
        {
            get { return _appBarColumnSpan; }
            set { SetProperty(ref _appBarColumnSpan, value); }
        }

        public PageViewModel()
        {
            ShellViewModel.SplitViewDisplayModeChanged += ((sender, e) => { UpdateCommonProperties(e); });
        }

        public virtual void UpdateCommonProperties(SplitViewDisplayMode splitViewDisplayMode)
        {
            if (splitViewDisplayMode == SplitViewDisplayMode.Overlay)
            {
                PageTitleMargin = new Thickness(69, 0, 12, 0);                
            }
            else
            {
                PageTitleMargin = new Thickness(21, 0, 12, 0);
                AppBarRow = 0;
                AppBarColumn = 1;
                AppBarColumnSpan = 1;
            }
        }
    }
}