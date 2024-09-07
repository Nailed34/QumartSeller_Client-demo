/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientServerConnection.Actions;
using System.Net.Http;
using System.Net.Http.Json;

namespace ClientWPF.Services
{
    /// <summary>
    /// Types of authorization result
    /// </summary>
    public enum EAuthorizationStatus
    {
        None,
        Authorized,
        Unauthorized
    }

    /// <summary>
    /// Types of server requests result
    /// </summary>
    public enum EServerRequestStatus
    {
        None,
        Success,
        BadUsernamePassword,
        ServerIsDown,
        ServerError
    }

    internal class AuthorizationService
    {
        /// <summary>
        /// Called when authorization status changed by send request
        /// </summary>
        public event Action<EAuthorizationStatus>? AuthorizationStatusChangedEvent;

        /// <summary>
        /// Status of last trying to authorize user
        /// </summary>
        public EServerRequestStatus LastRequestStatus
        {
            get => _lastRequestStatus; 
            private set => _lastRequestStatus = value;
        }
        private static EServerRequestStatus _lastRequestStatus = EServerRequestStatus.None;

        /// <summary>
        /// Current authorization status
        /// </summary>
        public EAuthorizationStatus AuthorizationStatus {
            get => _authorizationStatus;
            private set
            {
                _authorizationStatus = value;
                AuthorizationStatusChangedEvent?.Invoke(value);
            }
        }
        private static EAuthorizationStatus _authorizationStatus = EAuthorizationStatus.None;

        private static HttpClient HttpClient { get; set; }

        // Jwt token for connection protect, get by server on success authorization 
        public string Token
        {
            get => _token;
            private set => _token = value;
        }
        private static string _token = "";

        static AuthorizationService()
        {
            HttpClient = new HttpClient();
        }

        /// <summary>
        /// Send request with user data for get jwt token on success authorization, return new authorization status
        /// </summary>
        public void AuthorizeUser(string username, string password)
        {
            EServerRequestStatus newRequestStatus = EServerRequestStatus.None;
            EAuthorizationStatus newAuthorizationStatus = EAuthorizationStatus.None;

            Task.Run(async () =>
            {
                InUserAuthorization userAuthInfo = new InUserAuthorization { Username = username, Password = password };
                var userAuthInfoSerialized = JsonContent.Create(userAuthInfo);

                try
                {
                    var requestResult = await HttpClient.PostAsync(AppSettingsService.Settings.AuthUrl, userAuthInfoSerialized);
                    if (requestResult.IsSuccessStatusCode)
                    {
                        OutUserAuthorization userAuthResponse = await requestResult.Content.ReadFromJsonAsync<OutUserAuthorization>();
                        if (userAuthResponse.Success)
                        {
                            Token = userAuthResponse.Token;
                            newRequestStatus = EServerRequestStatus.Success;
                            newAuthorizationStatus = EAuthorizationStatus.Authorized;
                        }
                        else
                        {
                            newRequestStatus = EServerRequestStatus.BadUsernamePassword;
                            newAuthorizationStatus = EAuthorizationStatus.Unauthorized;
                        }
                    }
                    else
                    {
                        newRequestStatus = EServerRequestStatus.ServerError;
                        newAuthorizationStatus = EAuthorizationStatus.Unauthorized;
                    }
                }
                catch
                {
                    newRequestStatus = EServerRequestStatus.ServerIsDown;
                    newAuthorizationStatus = EAuthorizationStatus.Unauthorized;
                }
            }).ContinueWith(_ => 
            {
                LastRequestStatus = newRequestStatus;
                AuthorizationStatus = newAuthorizationStatus;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Change authorization status to unauthorized
        /// </summary>
        public void UnauthorizeUser()
        {
            AuthorizationStatus = EAuthorizationStatus.Unauthorized;
        }
    }
}
