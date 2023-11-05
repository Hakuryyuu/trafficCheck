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
using Android.OS;
using AndroidX.Core.App;
using MudBlazor;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trafficexposer.Data;

namespace trafficexposer.Platforms.Android
{
    [Service]
    internal class TrafficCheckBackgroundService : Service
    {
        DataProvider _dataProvider = new DataProvider();
        Timer _timer = null;
        int myId = (new object()).GetHashCode();
        int BadgeNumber = 0;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent,
            StartCommandFlags flags, int startId)
        {
            var input = intent.GetStringExtra("inputExtra");

            var notificationIntent = new Intent(this, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent,
                PendingIntentFlags.Immutable);

            var notification = new NotificationCompat.Builder(this,
                    MainApplication.ChannelId)
                .SetContentText(input)
                .SetSmallIcon(Resource.Drawable.AppIcon)
                .SetContentIntent(pendingIntent);

            _timer = new Timer(Timer_Elapsed, notification, 0, 60000);

            // You can stop the service from inside the service by calling StopSelf();

            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        void Timer_Elapsed(object state)
        {
            AndroidServiceManager.IsRunning = true;
            var message = String.Empty;

            if (Sysdba.SavedData.Areas != null)
            {
                if (Sysdba.SavedData.Areas.Length > 0)
                {
                    foreach (var area in Sysdba.SavedData.Areas)
                    {
                        var time = DateTime.Now.Subtract(area.EstimatedLeave.Value);

                        if (DateTime.Now.Hour == area.EstimatedLeave.Value.Hours &&
                            DateTime.Now.Minute == (area.EstimatedLeave.Value.Minutes - 5))
                        {
                            var incidents = _dataProvider.getIncidents(area).Result;
                            if (incidents.Length > 0)
                            {
                                if (Array.Exists(incidents, e => e.Severity == IncidentTypes.Severity.CLOSED)) // Quick overview info
                                {
                                    message = $"{area.StartLocation.Name} ➜ {area.Destiny.Name}: Road closure";
                                    SendNotification(ref state, ref message, 14024704);
                                }
                                else if (Array.Exists(incidents, e => e.Severity == IncidentTypes.Severity.STATIONARY_TRAFFIC))
                                {
                                    message = $"{area.StartLocation.Name} ➜ {area.Destiny.Name}: Stationary traffic";
                                    SendNotification(ref state, ref message, 14024704);
                                }
                                else if (Array.Exists(incidents, e => e.Severity == IncidentTypes.Severity.SLOW_TRAFFIC))
                                {
                                    message = $"{area.StartLocation.Name} ➜ {area.Destiny.Name}: Slow traffic";
                                    SendNotification(ref state, ref message, 16434517);
                                }
                                else if (Array.Exists(incidents, e => e.Severity == IncidentTypes.Severity.QUEUING_TRAFFIC))
                                {
                                    message = $"{area.StartLocation.Name} ➜ {area.Destiny.Name}: Queuing traffic";
                                    SendNotification(ref state, ref message, 16434517);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SendNotification(ref object state, ref string message, int color)
        {
            BadgeNumber++;

            if (LocalNotificationCenter.Current.AreNotificationsEnabled().Result == false)
            {
                LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "Traffic Notifier",
                Description = message,
                ReturningData = "Dummy data", // Returning data when tapped on notification.
            };
            LocalNotificationCenter.Current.Show(notification);

            //var notification = (NotificationCompat.Builder)state;
            //notification.SetNumber(BadgeNumber);
            //notification.SetContentTitle("Traffic Notifier");
            //notification.SetContentText(message);
            //notification.SetColor(color);
            //StartForeground(myId, notification.Build());
        }
    }
}
