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
using Android.Content.PM;
using Android.OS;
using trafficexposer.Platforms.Android;

namespace trafficexposer;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    | ConfigChanges.UiMode | ConfigChanges.ScreenLayout
    | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public MainActivity()
    {
        AndroidServiceManager.MainActivity = this;
    }

    public void StartService()
    {
        var serviceIntent = new Intent(this, typeof(TrafficCheckBackgroundService));
        serviceIntent.PutExtra("inputExtra", "Traffic Observer");
        StartService(serviceIntent);
    }

    public void StopService()
    {
        var serviceIntent = new Intent(this, typeof(TrafficCheckBackgroundService));
        StopService(serviceIntent);
    }
}
