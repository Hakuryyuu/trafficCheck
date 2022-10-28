using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    public class FileManagement
    {
        private readonly string APPATH;
        private readonly string SETTINGS_FILENAME;
        public FileManagement()
        {
            APPATH = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            SETTINGS_FILENAME = "\\Settings.json";
        }
        /// <summary>
        /// Save the current Settings to Drive
        /// </summary>
        /// <param name="oData"></param>
        /// <returns></returns>
        public async Task SerializeSettings(Settings oData)
        {
            string sJson = JsonConvert.SerializeObject(oData);
            await WriteSettingsToDisk(sJson);
        }
        private async Task WriteSettingsToDisk(string sJson)
        {
            try
            {
               await File.WriteAllTextAsync(APPATH + SETTINGS_FILENAME, sJson);
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
            string sSettingsJson = File.ReadAllText(APPATH + SETTINGS_FILENAME); 
            return JsonConvert.DeserializeObject<Settings>(sSettingsJson);
        }
    }
}
