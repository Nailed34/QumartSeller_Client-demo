/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using Microsoft.AspNetCore.SignalR.Client;

namespace ClientWPF.Services
{
    /// <summary>
    /// Types of connection result
    /// </summary>
    public enum EConnectionStatus
    {
        None,
        Connected,
        Disconnected
    }

    internal class ConnectionService
    {
        /// <summary>
        /// Called when connection with server lost, return reason
        /// </summary>
        public event Action<Exception>? ConnectionLostEvent;
        /// <summary>
        /// Called when connection lost and started auto reconnecting, return reason
        /// </summary>
        public event Action<Exception>? ConnectionStartAutoReconnectingEvent;
        /// <summary>
        /// Called when autoreconnecting success, return new connection id or empty
        /// </summary>
        public event Action<string>? ConnectionAutoReconnectedEvent;
        /// <summary>
        /// Called when connection status was changed
        /// </summary>
        public event Action<EConnectionStatus>? ConnectionStatusChangedEvent;

        /// <summary>
        /// Current connection status
        /// </summary>
        public EConnectionStatus ConnectionStatus
        { 
            get => _connectionStatus; 
            private set 
            { 
                _connectionStatus = value; ConnectionStatusChangedEvent?.Invoke(value);
            }
        }
        private static EConnectionStatus _connectionStatus = EConnectionStatus.None;

        public static HubConnection? Connection { get; private set; }

        public ConnectionService()
        {
            SetupBindings();
        }

        /// <summary>
        /// Connect to the server by setup new hub connection
        /// </summary>
        public void Connect(string token)
        {
            EConnectionStatus newStatus = EConnectionStatus.None;
            Task.Run(async () =>
            {
                if (ConnectionStatus != EConnectionStatus.Connected && token != "")
                {
                    try
                    {
                        SetupConnection(token);
                        if (Connection != null)
                            await Connection.StartAsync();
                        newStatus = EConnectionStatus.Connected;
                    }
                    catch
                    {
                        newStatus = EConnectionStatus.Disconnected;
                    }
                }
            }).ContinueWith(_ =>
            {
                ConnectionStatus = newStatus;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Create connection with token and bind events by new connection
        private void SetupConnection(string token)
        {
            // Create connection with tocken
            Connection = new HubConnectionBuilder().WithUrl(AppSettingsService.Settings.ConnectionUrl,
                options =>
                {
                    #pragma warning disable CS8619
                    options.AccessTokenProvider = () => Task.FromResult(token);
                    #pragma warning restore CS8619
                }).WithAutomaticReconnect().Build();

            SetupBindings();
        }

        // Bind events by connection
        private void SetupBindings()
        {
            if (Connection != null)
            {
                // Bind event for connection lost
                Connection.Closed += (exception) =>
                {
                    ConnectionStatus = EConnectionStatus.Disconnected;
                    ConnectionLostEvent?.Invoke(exception ?? new Exception());
                    return Task.CompletedTask;
                };

                // Bind event for auto reconnect
                Connection.Reconnecting += (exception) =>
                {
                    ConnectionStartAutoReconnectingEvent?.Invoke(exception ?? new Exception());
                    return Task.CompletedTask;
                };

                // Bind event for auto reconnected
                Connection.Reconnected += (NewConnectionId) =>
                {
                    ConnectionAutoReconnectedEvent?.Invoke(NewConnectionId ?? "");
                    return Task.CompletedTask;
                };
            }
        }
    }
}
