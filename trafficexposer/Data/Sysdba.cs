using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Data
{
    /// <summary>
    /// Static Runtime Data and other Constants
    /// </summary>
    public static class Sysdba
    {
        static FileManagement FM = new FileManagement();
        /// <summary>
        /// TomTom API Key
        /// </summary>
        public const string API_KEY = "Kl1LK6P8HUHvO2y1CYnIaQ5P25v9b3h7";

        /// <summary>
        /// Saved Areas from the User
        /// </summary>
        public static Settings SavedData { get; set; } = FM.getSettings().Result;
    }
}
