/*
*  Created by: Hakuryuu
*  www.hakuryuu.net
*  info@hakuryuu.net
*
*  Copyright (c) 2023 Hakuryuu
*
*/

using Android.App;
using Android.Content;
using Android.Widget;
using AndroidX.Core.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trafficexposer.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                //Toast.MakeText(context, "[DEBUG] Boot completed event received",
                //    ToastLength.Short).Show();

                var serviceIntent = new Intent(context,
                    typeof(TrafficCheckBackgroundService));

                ContextCompat.StartForegroundService(context,
                    serviceIntent);
            }
        }
    }
}
