using System;
using AppStudio.DataProviders.Menu;
using AppStudio.Uwp.Navigation;

namespace Windows10News
{
    public static class MenuExtensions
    {
        public static NavigationInfo ToNavigationInfo(this MenuSchema menuItem, bool includeState = false)
        {
            if (menuItem == null)
            {
                return null;
            }

            var navigationInfo = new NavigationInfo
            {
                NavigationType = SafeParse(menuItem.MenuType),
                IncludeState = includeState
            };

            if (navigationInfo.NavigationType == NavigationType.Page)
            {
                navigationInfo.TargetPage = menuItem.Target;
            }
            else
            {
                navigationInfo.TargetUri = new Uri(menuItem.Target, UriKind.Absolute);
            }

            return navigationInfo;
        }

        private static NavigationType SafeParse(string value)
        {
            var type = NavigationType.Page;
            Enum.TryParse(value, out type);

            return type;
        }
    }
}
