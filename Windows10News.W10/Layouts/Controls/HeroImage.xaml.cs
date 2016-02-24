using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Windows10News.Layouts.Controls
{
    public sealed partial class HeroImage : UserControl, INotifyPropertyChanged
    {
	public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(string), typeof(HeroImage), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty MobileImageProperty =
            DependencyProperty.Register("MobileImage", typeof(string), typeof(HeroImage), new PropertyMetadata(string.Empty));

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public string MobileImage
        {
            get { return (string)GetValue(MobileImageProperty); }
            set { SetValue(MobileImageProperty, value); }
        }
        public string ImageSource
        {
            get
            {
                if (Window.Current.Bounds.Width < 500)
                {
                    if(!string.IsNullOrEmpty(MobileImage))
                    {
                        return MobileImage;
                    }
                    else
                    {
                        return Image;
                    }
                }
                else
                {
                    return Image;
                }
            }
        }
        public Stretch ImageStretch
        {
            get
            {
                if (Window.Current.Bounds.Width >= 500 && Window.Current.Bounds.Width <= 800)
                {
                    return Stretch.UniformToFill;
                }
                else
                {
                    if (string.IsNullOrEmpty(MobileImage))
                    {
                        return Stretch.UniformToFill;
                    }
                    else
                    {
                        return Stretch.Uniform;
                    }
                }
            }
        }
        public HeroImage()
        {
            this.InitializeComponent();
        }
		public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += ControlSizeChanged;
        }

        private void ControlSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            OnPropertyChanged("ImageSource");
            OnPropertyChanged("ImageStretch");
        }

        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= ControlSizeChanged;
        }
    }
}
