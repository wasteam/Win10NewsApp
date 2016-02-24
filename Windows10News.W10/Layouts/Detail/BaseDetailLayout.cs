using System.Collections.Generic;
using System.ComponentModel;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows10News.Services;
using Windows10News.ViewModels;

namespace Windows10News.Layouts.Detail
{
    public abstract class BaseDetailLayout : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(DetailViewModel), typeof(BaseDetailLayout), new PropertyMetadata(null));

        public static readonly DependencyProperty BodyFontSizeProperty
            = DependencyProperty.Register("BodyFontSize", typeof(int), typeof(BaseDetailLayout), new PropertyMetadata(0));

        public BaseDetailLayout()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DetailViewModel ViewModel
        {
            get { return (DetailViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public int BodyFontSize
        {
            get { return (int)GetValue(BodyFontSizeProperty); }
            set { SetValue(BodyFontSizeProperty, value); }
        }

        public void OnPropertyChanged(string propertyName)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
