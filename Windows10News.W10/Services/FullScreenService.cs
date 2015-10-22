using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Windows10News.Services
{
    public static class FullScreenService
    {
        public static bool CurrentPageSupportFullScreen;
        public static event EventHandler<bool> FullScreenModeChanged;
        public static event EventHandler FullScreenPlayActionsChanged;
        public static void EnterFullScreenMode(bool playActions = false)
        {
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            NotifyFullScreenModeChangedEvent(true);
            if (playActions)
            {
                NotifyFullScreenPlayActionsChanged();
            }
        }
        public static void ExitFullScreenMode()
        {
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            NotifyFullScreenModeChangedEvent(false);
        }
        private static void NotifyFullScreenModeChangedEvent(bool isFullScreenMode)
        {
            if (FullScreenModeChanged != null)
            {
                FullScreenModeChanged(null, isFullScreenMode);
            }
        }
        private static void NotifyFullScreenPlayActionsChanged()
        {
            if (FullScreenPlayActionsChanged != null)
            {
                FullScreenPlayActionsChanged(null, EventArgs.Empty);
            }
        }
    }
}