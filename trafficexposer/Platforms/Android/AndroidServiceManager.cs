 /*
 *  Created by: Hakuryuu
 *  www.hakuryuu.net
 *  info@hakuryuu.net
 *
 *  Copyright (c) 2023 Hakuryuu
 *
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trafficexposer.Data;

namespace trafficexposer.Platforms.Android
{
    public static class AndroidServiceManager
    {
        public static MainActivity MainActivity { get; set; }

        public static bool IsRunning { get; set; }

        public static void StartMyService()
        {
            FileManagement _fm = new FileManagement();
            if (Sysdba.SavedData == null)
            {
                Sysdba.SavedData = _fm.getSettings().Result;
            }
            if (MainActivity == null) return;
            MainActivity.StartService();
        }

        public static void StopMyService()
        {
            if (MainActivity == null) return;
            MainActivity.StopService();
            IsRunning = false;
        }
    }
}
