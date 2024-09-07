/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Services;
using ClientWPF.ViewModels.Connection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ClientWPF.ViewModels.Authorization
{
    internal partial class AuthorizationViewModel : ObservableObject
    {
        const int MIN_USERNAME_LENGTH = 5;
        const int MAX_USERNAME_LENGTH = 30;
        const int MIN_PASSWORD_LENGTH = 8;
        const int MAX_PASSWORD_LENGTH = 30;

        private readonly AuthorizationService _authorizationService = new();
        private readonly UserDataService _userDataService = new();

        [RelayCommand]
        private void TryToAuthorize()
        {  
            if (Username.Length < MIN_USERNAME_LENGTH)
            {
                ErrorMessage = "Имя пользователя должно быть более " + MIN_USERNAME_LENGTH + " символов";
                return;
            }

            if (Username.Length > MAX_USERNAME_LENGTH)
            {
                ErrorMessage = "Имя пользователя должно быть менее " + MAX_USERNAME_LENGTH + " символов";
                return;
            }

            if (Password.Length < MIN_PASSWORD_LENGTH)
            {
                ErrorMessage = "Длина пароля не может быть менее " + MIN_PASSWORD_LENGTH + " символов";
                return;
            }

            if (Password.Length > MAX_PASSWORD_LENGTH)
            {
                ErrorMessage = "Длина пароля не может быть более " + MIN_PASSWORD_LENGTH + " символов";
                return;
            }

            IsInputEnabled = false;
            _authorizationService.AuthorizationStatusChangedEvent += OnAuthorizationStatusChanged;
            _authorizationService.AuthorizeUser(Username, Password);
        }

        private void OnAuthorizationStatusChanged(EAuthorizationStatus status)
        {
            IsInputEnabled = true;
            _authorizationService.AuthorizationStatusChangedEvent -= OnAuthorizationStatusChanged;

            // Success
            if (status == EAuthorizationStatus.Authorized)
            {
                ErrorMessage = "";

                // Save auth data
                _userDataService.Username = Username;
                _userDataService.Password = Password;
                _userDataService.SaveUserData();

                // Change to next connection page
                //_regionManager.RequestNavigate("PageRegion", nameof(ConnectionView), (exc) =>
                //{
                //    if (!exc.Success)
                //    {
                //        ErrorMessage = "Внутренняя ошибка программы";
                //    }
                //});
                NavigationService.Navigate<ConnectionViewModel>("FrameMain");
            }
            else
            {
                switch (_authorizationService.LastRequestStatus)
                {
                    case EServerRequestStatus.BadUsernamePassword:
                        ErrorMessage = "Неверное имя пользователя или пароль";
                        break;
                    case EServerRequestStatus.ServerIsDown:
                        ErrorMessage = "Сервер недоступен";
                        break;
                    case EServerRequestStatus.ServerError:
                        ErrorMessage = "Произошла ошибка запроса";
                        break;
                    default:
                        ErrorMessage = "Неизвестная ошибка";
                        break;
                }
            }
        }

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private string _username = "";

        public string Password { private get; set; } = "";

        [ObservableProperty]
        private bool _isInputEnabled = true;
    }
}