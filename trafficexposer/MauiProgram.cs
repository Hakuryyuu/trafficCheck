/*
*  Created by: Hakuryuu
*  www.hakuryuu.net
*  info@hakuryuu.net
*
*  Copyright (c) 2023 Hakuryuu
*
*/

using Microsoft.AspNetCore.Components.WebView.Maui;
using MudBlazor.Services;
using Plugin.LocalNotification;
using trafficexposer.Data;

namespace trafficexposer;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
		

        builder.Services.AddMudServices();
        builder.Services.AddSingleton<DataProvider>();

        return builder.Build();
	}
}
