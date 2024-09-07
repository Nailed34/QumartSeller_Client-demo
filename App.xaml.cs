/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Services;
using System.Windows;
using ClientWPF.Views;
using ClientWPF.ViewModels;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppSettingsService.InitSettings();
            PhotoCacheService.RunPhotoCache();

            MainWindow = new MainView()
            {
                DataContext = new MainViewModel()
            };
            MainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {            
            _ = ConnectionService.Connection?.StopAsync();
            PhotoCacheService.SavePhotoDataAndClose();
        }
    }

}
