using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data.Helpers
{
    public class FileManagement
    {
        private readonly string APPATH;
        private readonly string SETTINGS_FILENAME;
        public FileManagement(string Path)
        {
            APPATH = Path;
            SETTINGS_FILENAME = "Settings.json";
        }
        /// <summary>
        /// Save the current Settings to Drive
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SerializeSettings(Settings data)
        {
            string Json = JsonConvert.SerializeObject(data);
            await WriteSettingsToDisk(Json);
        }
        private async Task WriteSettingsToDisk(string Json)
        {
            try
            {
               await File.WriteAllTextAsync(APPATH + SETTINGS_FILENAME, Json);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Read last saved Settings
        /// </summary>
        /// <returns></returns>
        public async Task<Settings> getSettings()
        {
            if (!File.Exists(APPATH + SETTINGS_FILENAME))
            {
                await SerializeSettings(new Settings());
            }
            string SettingsJson = await ReadSettingsFromDisk();
            return JsonConvert.DeserializeObject<Settings>(SettingsJson);
        }
        private async Task<string> ReadSettingsFromDisk()
        {
            return await File.ReadAllTextAsync(APPATH + SETTINGS_FILENAME);
        }
    }
}
