using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace Windows10News.Triggers
{
    public class IsTypePresentStateTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty TypeNameProperty =
            DependencyProperty.Register("TypeName", typeof(string), typeof(IsTypePresentStateTrigger),
            new PropertyMetadata(null, OnTypeNamePropertyChanged));

        public string TypeName
        {
            get { return (string)GetValue(TypeNameProperty); }
            set { SetValue(TypeNameProperty, value); }
        }

        private static void OnTypeNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as IsTypePresentStateTrigger;

            var triggerResult = false;
            var val = e.NewValue as string;

            if (!string.IsNullOrWhiteSpace(val))
            {
                triggerResult = ApiInformation.IsTypePresent(val);
            }
            trigger.SetActive(triggerResult);
        }
    }
}
