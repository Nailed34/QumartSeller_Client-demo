/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using System.Configuration;

namespace ClientWPF.Services
{
    internal class AppSettings
    {
        public string AuthUrl { get; set; } = "http://localhost:5000/auth";
        public string ConnectionUrl { get; set; } = "http://localhost:5000/main";
    }

    internal static class AppSettingsService
    {
        public static AppSettings Settings { get; set; } = new();

        public static void InitSettings()
        {
            Settings.AuthUrl = ConfigurationManager.AppSettings["authUrl"] ?? "http://localhost:5000/auth";
            Settings.ConnectionUrl = ConfigurationManager.AppSettings["connectionUrl"] ?? "http://localhost:5000/main";
        }
    }
}
