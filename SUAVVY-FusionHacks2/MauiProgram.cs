﻿using Microsoft.Extensions.Logging;
using SUAVVY_FusionHacks2.Data;

namespace SUAVVY_FusionHacks2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AppShellContext>();
            builder.Services.AddSingleton<DatabaseContext>();
            return builder.Build();
        }
    }
}
