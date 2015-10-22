using Windows.UI.Xaml;

namespace Windows10News.Triggers
{
    public class PlatformStateTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty PlatformTypeProperty =
            DependencyProperty.Register("PlatformType", typeof(PlatformType), typeof(PlatformStateTrigger),
            new PropertyMetadata(PlatformType.Unknown, OnPlatformTypePropertyChanged));

        public PlatformType PlatformType
        {
            get { return (PlatformType)GetValue(PlatformTypeProperty); }
            set { SetValue(PlatformTypeProperty, value); }
        }

        public static PlatformType ParseType(string value)
        {
            var parsedValue = PlatformType.Unknown;
            PlatformType.TryParse(value, out parsedValue);

            return parsedValue;
        }

        private static void OnPlatformTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as PlatformStateTrigger;
            if (e.NewValue != null)
            {
                var selectedPlatform = ParseType(e.NewValue.ToString());

                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                var triggerResult = qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"].ToLowerInvariant() == selectedPlatform.ToString().ToLowerInvariant();

                trigger.SetActive(triggerResult);
            }
        }
    }

    public enum PlatformType
    {
        Desktop,
        Mobile,
        Unknown
    }
}
