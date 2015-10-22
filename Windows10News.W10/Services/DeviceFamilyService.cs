using System;
using Windows.Foundation.Collections;

namespace Windows10News.Services
{
    public static class DeviceFamilyService
    {
        private static IObservableMap<String, String> _qualifierValues;
        private static IObservableMap<String, String> QualifierValues
        {
            get
            {
                if (_qualifierValues == null)
                {
                    _qualifierValues = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;

                }
                return _qualifierValues;
            }
        }
        public static bool IsMobile
        {
            get
            {
                return QualifierValues.ContainsKey("DeviceFamily") && QualifierValues["DeviceFamily"].ToLowerInvariant() == "Mobile".ToLowerInvariant();
            }
        }
    }
}
