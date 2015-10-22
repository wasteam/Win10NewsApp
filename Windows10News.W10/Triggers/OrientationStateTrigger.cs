using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;

namespace Windows10News.Triggers
{
    public class OrientationStateTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientations), typeof(OrientationStateTrigger),
            new PropertyMetadata(Orientations.None, OnOrientationPropertyChanged));
        public Orientations Orientation
        {
            get { return (Orientations)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public OrientationStateTrigger()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                DisplayInformation.GetForCurrentView().OrientationChanged += ((sender, args) =>
                {
                    UpdateTrigger(sender.CurrentOrientation);
                });
            }
        }

        private void UpdateTrigger(Windows.Graphics.Display.DisplayOrientations orientation)
        {
            var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
            var isOnMobile = qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"].ToLowerInvariant() == "Mobile".ToLowerInvariant();
            if (orientation == Windows.Graphics.Display.DisplayOrientations.None)
            {
                SetActive(false);
            }
            else if (orientation == Windows.Graphics.Display.DisplayOrientations.Landscape ||
               orientation == Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped)
            {
                if (isOnMobile)
                {
                    SetActive(Orientation == Orientations.LandscapeMobile);
                }
                else
                {
                    SetActive(Orientation == Orientations.Landscape);
                }
            }
            else if (orientation == Windows.Graphics.Display.DisplayOrientations.Portrait ||
                    orientation == Windows.Graphics.Display.DisplayOrientations.PortraitFlipped)
            {
                if (isOnMobile)
                {
                    SetActive(Orientation == Orientations.PortraitMobile);
                }
                else
                {
                    SetActive(Orientation == Orientations.Portrait);
                }
            }
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as OrientationStateTrigger;
            if (trigger != null)
            {
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    var orientation = DisplayInformation.GetForCurrentView().CurrentOrientation;
                    trigger.UpdateTrigger(orientation);
                }
            }
        }

        public enum Orientations
        {
            None,
            Landscape,
            Portrait,
            LandscapeMobile,
            PortraitMobile

        }
    }
}
