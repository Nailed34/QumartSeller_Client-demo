/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Services;
using ClientWPF.ViewModels.Connection;
using ClientWPF.ViewModels.Global.Products;
using ClientWPF.ViewModels.Global.Settings;
using ClientWPF.Views.Global.Products;
using ClientWPF.Views.Global.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ClientWPF.ViewModels.Global
{
    internal partial class GlobalViewModel : ObservableObject
    {   
        private readonly ConnectionService _connectionService = new();

        private readonly TaskScheduler _taskScheduler;

        public GlobalViewModel()
        {
            //_regionManager.RegisterViewWithRegion<ProductsView>("GlobalPageRegion");
            NavigationService.RegisterLocator<ProductsViewModel, ProductsView>();
            NavigationService.RegisterLocator<SettingsViewModel, SettingsView>();

            NavigationService.Navigate<ProductsViewModel>("FrameGlobal");

            _taskScheduler = TaskScheduler.Current;

            _connectionService.ConnectionStartAutoReconnectingEvent += OnConnectionStartAutoReconnecting;
            _connectionService.ConnectionAutoReconnectedEvent += OnConnectionAutoReconnected;
            _connectionService.ConnectionLostEvent += OnConnectionLost;
        }

        private void OnConnectionStartAutoReconnecting(Exception obj)
        {
            IsAutoreconnecting = true;
        }

        private void OnConnectionAutoReconnected(string obj)
        {
            IsAutoreconnecting = false;
        }

        private void OnConnectionLost(Exception obj)
        {
            IsAutoreconnecting = false;
            NavigateToConnectionView();
        }

        private void NavigateToConnectionView()
        {
            var navigateTask = new Task(() =>
            {
                //_regionManager.RequestNavigate("PageRegion", nameof(ConnectionView));
                NavigationService.Navigate<ConnectionViewModel>("FrameMain");
            });

            navigateTask.RunSynchronously(_taskScheduler);
        }

        [RelayCommand]
        private void SwitchPage(string? pageName)
        {
            if (pageName == null)
                return;

            switch(pageName)
            {
                case nameof(ProductsView):
                    IsProductsButtonEnabled = false;
                    IsSettingsButtonEnabled = true;
                    NavigationService.Navigate<ProductsViewModel>("FrameGlobal");
                    break;
                case nameof(SettingsView):
                    IsProductsButtonEnabled = true;
                    IsSettingsButtonEnabled = false;
                    NavigationService.Navigate<SettingsViewModel>("FrameGlobal");
                    break;
                default:
                    return;
            }

            //_regionManager.RequestNavigate("GlobalPageRegion", pageName);
        }

        [ObservableProperty]
        private bool _isProductsButtonEnabled = false;

        [ObservableProperty]
        private bool _isSettingsButtonEnabled = true;

        [ObservableProperty]
        private bool _isAutoreconnecting = false;
    }
}
