using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using trafficexposer.Data.Helpers;

namespace trafficexposer.Data
{
    /// <summary>
    /// Static Runtime Data and other Constants
    /// </summary>
    public static class Sysdba
    {
        static FileManagement FM = new FileManagement(APPLICATION_PATH);
        /// <summary>
        /// TomTom API Key
        /// </summary>
        public const string API_KEY = "";
        /// <summary>
        /// Application Path for saving Data
        /// </summary>      
        public static readonly string APPLICATION_PATH = System.Reflection.Assembly.GetExecutingAssembly().Location;

        /// <summary>
        /// Saved Areas from the User
        /// </summary>
        public static Settings SavedData { get; set; } = FM.getSettings().Result;
    }
}
