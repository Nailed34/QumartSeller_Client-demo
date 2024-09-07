/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Services;
using ClientWPF.ViewModels.Authorization;
using ClientWPF.ViewModels.Global;
using ClientWPF.Views.Authorization;
using ClientWPF.Views.Global;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ClientWPF.ViewModels.Connection
{
    internal partial class ConnectionViewModel : ObservableObject
    {
        private readonly UserDataService _userDataService = new();
        private readonly AuthorizationService _authorizationService = new();
        private readonly ConnectionService _connectionService = new();

        public ConnectionViewModel()
        {
            _connectionService.ConnectionLostEvent += OnConnectionLostEvent;

            Reconnect();
        }

        private void OnConnectionLostEvent(Exception obj)
        {
            ErrorText = "Соединение с сервером потеряно";
            IsError = true;
        }

        [RelayCommand]
        private void Reconnect()
        {
            // Check user data status or success trying to load
            if (_userDataService.UserDataLoadStatus == EUserDataLoadStatus.Loaded || _userDataService.LoadUserData() == EUserDataLoadStatus.Loaded)
            {
                // Check auth status
                if (_authorizationService.AuthorizationStatus == EAuthorizationStatus.Authorized)
                {
                    // Try to connect
                    IsError = false;
                    _connectionService.ConnectionStatusChangedEvent += OnConnectionStatusChanged;
                    _connectionService.Connect(_authorizationService.Token);
                }
                else
                {
                    // Try to authorize
                    IsError = false;
                    _authorizationService.AuthorizationStatusChangedEvent += OnAuthorizationStatusChanged;
                    _authorizationService.AuthorizeUser(_userDataService.Username, _userDataService.Password);
                }
            }          
            else
            {
                ErrorText = "Ошибка входа, вернитесь на страницу авторизации";
                IsError = true;
            }
        }

        private void OnConnectionStatusChanged(EConnectionStatus newStatus)
        {
            _connectionService.ConnectionStatusChangedEvent -= OnConnectionStatusChanged;

            if (newStatus == EConnectionStatus.Connected)
            {
                ErrorText = "";
                //_regionManager.RequestNavigate("PageRegion", nameof(GlobalView), (exc) =>
                //{
                //    if (!exc.Success)
                //    {
                //        ErrorText = "Ошибка навигации";
                //        IsError = true;
                //    }
                //});
                NavigationService.Navigate<GlobalViewModel>("FrameMain");
            }    
            else
            {
                ErrorText = "Не удалось создать соединение";
                IsError = true;
            }
        }

        private void OnAuthorizationStatusChanged(EAuthorizationStatus newStatus)
        {
            _authorizationService.AuthorizationStatusChangedEvent -= OnAuthorizationStatusChanged;

            if (newStatus == EAuthorizationStatus.Authorized)
            {
                _connectionService.ConnectionStatusChangedEvent += OnConnectionStatusChanged;
                _connectionService.Connect(_authorizationService.Token);
            }
            else
            {
                switch (_authorizationService.LastRequestStatus)
                {
                    case EServerRequestStatus.BadUsernamePassword:
                        ErrorText = "Неверные данные для входа, вернитесь на страницу авторизации";
                        break;
                    case EServerRequestStatus.ServerIsDown:
                        ErrorText = "Сервер недоступен";
                        break;
                    case EServerRequestStatus.ServerError:
                        ErrorText = "Ошибка запроса";
                        break;
                    default:
                        ErrorText = "Внутренняя ошибка программы";
                        break;
                }
                IsError = true;
            }
        }

        [RelayCommand]
        private void Return()
        {
            //_regionManager.RequestNavigate("PageRegion", nameof(AuthorizationView));
            NavigationService.Navigate<AuthorizationViewModel>("FrameMain");
        }

        [ObservableProperty]
        private string _errorText = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsConnection))]
        private bool _isError = false;

        public bool IsConnection => !IsError;
    }
}
