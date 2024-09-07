/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientServerConnection;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ClientWPF.Models
{
    public partial class ProductCard : ObservableObject
    {
        public ProductCard() { }

        public void ChangeSource(ProductInfo? cardSource)
        {
            if (cardSource != null)
            {
                Id = cardSource.id;
                Name = cardSource.name;
                Photo = cardSource.photo;
                Stocks = cardSource.stocks;
                Articuls.Clear();
                foreach (var articul in cardSource.articuls)
                    Articuls.Add(articul);
                Barcodes.Clear();
                foreach (var barcode in cardSource.barcodes)
                    Barcodes.Add(barcode);
                Marketplaces.Clear();
                foreach (var marketplace in cardSource.marketplaces)
                    Marketplaces.Add(marketplace);
                Details.Clear();
                foreach (var detail in cardSource.cards)
                    Details.Add(detail);
                IsDisplay = true;
            }
            else
            {
                Id = "";
                Name = "";
                Photo = "";
                Stocks = 0;
                Articuls.Clear();
                Barcodes.Clear();
                Marketplaces.Clear();
                Details.Clear();
                IsDisplay = false;
            }
        }

        /// <summary>
        /// Onion id from DB
        /// </summary>
        [ObservableProperty]
        private string _id = "";

        /// <summary>
        /// Name of onion determines by priority marketplace
        /// </summary>
        [ObservableProperty]
        private string _name = "";

        /// <summary>
        /// Photo of product determines by priority marketplace
        /// </summary>
        [ObservableProperty]
        private string _photo = "";

        /// <summary>
        /// General stocks for child cards
        /// </summary>
        [ObservableProperty]
        private int _stocks = 0;

        /// <summary>
        /// Display this card or collapse
        /// </summary>
        [ObservableProperty]
        private bool _isDisplay = false;

        /// <summary>
        /// List of child cards articuls
        /// </summary>
        public ObservableCollection<string> Articuls { get; set; } = new();

        /// <summary>
        /// List of child cards barcodes
        /// </summary>
        public ObservableCollection<string> Barcodes { get; set; } = new();

        /// <summary>
        /// List of child cards marketplaces
        /// </summary>
        public ObservableCollection<EMarketplaces> Marketplaces { get; set; } = new();

        /// <summary>
        /// Details about including cards
        /// </summary>
        public ObservableCollection<ProductInfoCard> Details { get; set; } = new();
    }
}
