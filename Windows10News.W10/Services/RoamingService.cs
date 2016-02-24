using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Windows10News.Services
{
    public class RoamingService
    {
        public async Task SaveAsync<T>(string name, T content)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            StorageFile roamingFile = await roamingFolder.CreateFileAsync($"{name}.json", CreationCollisionOption.ReplaceExisting);

            var fileContent = JsonConvert.SerializeObject(content, Formatting.None);

            await FileIO.WriteTextAsync(roamingFile, fileContent);
        }

        public async Task<T> GetAsync<T>(string name)
        {
            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            try
            {
                StorageFile roamingFile = await roamingFolder.GetFileAsync($"{name}.json");
                var fileContent = await FileIO.ReadTextAsync(roamingFile);
                return JsonConvert.DeserializeObject<T>(fileContent);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }
    }
}
