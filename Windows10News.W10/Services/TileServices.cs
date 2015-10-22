using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace Windows10News.Services
{
    public class TileServices
    {
        public static async Task CreateLiveTile(string tileFileName)
        {
            if (tileFileName != null)
            {
                try
                {
                    var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                    tileUpdater.EnableNotificationQueue(true);
                    tileUpdater.Clear();

                    StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    StorageFile file = await folder.GetFileAsync(tileFileName);

                    tileUpdater.Update(new TileNotification(await XmlDocument.LoadFromFileAsync(file)));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
