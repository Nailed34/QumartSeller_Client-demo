/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Services;
using ClientWPF.ViewModels.Authorization;
using ClientWPF.ViewModels.Connection;
using ClientWPF.ViewModels.Global;
using ClientWPF.Views.Authorization;
using ClientWPF.Views.Connection;
using ClientWPF.Views.Global;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClientWPF.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        private UserDataService _userDataService = new();

        public MainViewModel()
        {
            // Register locators
            NavigationService.RegisterLocator<AuthorizationViewModel, AuthorizationView>();
            NavigationService.RegisterLocator<ConnectionViewModel, ConnectionView>();
            NavigationService.RegisterLocator<GlobalViewModel, GlobalView>();

            if (_userDataService.LoadUserData() == EUserDataLoadStatus.Loaded)
                NavigationService.Navigate<ConnectionViewModel>("FrameMain");
            else
                NavigationService.Navigate<AuthorizationViewModel>("FrameMain");
        }
	}
}
