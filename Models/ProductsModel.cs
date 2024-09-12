/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientServerConnection.Actions;
using ClientServerConnection;
using ClientWPF.Services;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.SignalR.Client;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClientWPF.Models
{
    internal partial class ProductsModel : ObservableObject
    {
        public const int MAX_CARDS_IN_PAGE = 50;

        private readonly ConnectionService _connectionService = new();

        // Save context for do actions in right thread
        private readonly TaskScheduler _taskScheduler;

        /// <summary>
        /// Collection with current products to display
        /// </summary>
        public readonly ReadOnlyObservableCollection<ProductCard> Products;
        private readonly ObservableCollection<ProductCard> _products = new();

        /// <summary>
        /// Count of products from request
        /// </summary>
        [ObservableProperty]
        private int _productsCount = 0;

        public ProductsModel()
        {
            // Save context
            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // Create readonly collection for UI
            Products = new(_products);

            // Create cards for show products info
            GenerateCards();

            // Bind responses from server on connection
            if (_connectionService.ConnectionStatus == EConnectionStatus.Connected)
                BindResponses();

            // Rebind responses on change connection
            _connectionService.ConnectionStatusChangedEvent += (newStatus) => 
            {
                if (newStatus == EConnectionStatus.Connected)
                    BindResponses();
            };          
        }

        private void GenerateCards()
        {
            for (int i = 0; i < MAX_CARDS_IN_PAGE; i++)
                _products.Add(new ProductCard());
        }

        private void BindResponses()
        {
            ConnectionService.Connection?.On<OutGetProductsInfo>("OutGetProductsInfo", OutGetProductsInfo);
        }

        /// <summary>
        /// Do request for get products info without filter
        /// </summary>
        public void InGetProductsInfo(int firstIndex, int lastIndex)
        {
            if (ConnectionService.Connection != null && _connectionService.ConnectionStatus == EConnectionStatus.Connected)
                ConnectionService.Connection.SendAsync("InGetProductsInfo", new InGetProductsInfo() { FirstIndex = firstIndex, LastIndex = lastIndex });
        }

        private void OutGetProductsInfo(OutGetProductsInfo response)
        {
            var synchTask = new Task(() =>
            {
                var products = ProductInfo.DeserializeProductInfo(response.ProductsInfo);

                // Update displayed products
                for (int i = 0; i < products.Count; i++)
                    _products[i].ChangeSource(products[i]);

                // Collapse empty products
                for (int i = products.Count; i < MAX_CARDS_IN_PAGE; i++)
                    _products[i].ChangeSource(null);

                // Setup count
                ProductsCount = response.ProductsCount;

            });
            synchTask.RunSynchronously(_taskScheduler);
        }

        /// <summary>
        /// Do request for search products
        /// </summary>
        public void InSearchProducts(string searchRequest, int firstIndex, int lastIndex)
        {
            if (ConnectionService.Connection != null && _connectionService.ConnectionStatus == EConnectionStatus.Connected)
                ConnectionService.Connection.SendAsync("InSearchProducts", new InSearchProducts() { SearchRequest = searchRequest, FirstIndex = firstIndex, LastIndex= lastIndex });
        }

        private void OutSearchProducts(OutSearchProducts response)
        {
            var synchTask = new Task(() =>
            {
                var products = ProductInfo.DeserializeProductInfo(response.FoundProducts);

                // Update displayed products
                for (int i = 0; i < products.Count; i++)
                    _products[i].ChangeSource(products[i]);

                // Collapse empty products
                for (int i = products.Count; i < MAX_CARDS_IN_PAGE; i++)
                    _products[i].ChangeSource(null);

                // Setup count
                ProductsCount = response.FoundProductsCount;
            });
            synchTask.RunSynchronously(_taskScheduler);
        }
    }
}
