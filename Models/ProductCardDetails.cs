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
    public partial class ProductCardDetails : ObservableObject
    {
        public ProductCardDetails() { }

        public void ChangeSource(ProductInfoCard? detailsSource)
        {
            if (detailsSource != null)
            {
                Articul = detailsSource.articul;
                Name = detailsSource.name;
                Stocks = detailsSource.stocks;
                CreationDate = detailsSource.creation_date;
                IsSynch = detailsSource.is_synch;
                Multiplicity = detailsSource.multiplicity;
                Marketplace = detailsSource.marketplace;
                Barcodes.Clear();
                foreach (var barcode in detailsSource.barcodes)
                    Barcodes.Add(barcode);
                IsDisplay = true;
            }
            else
            {
                Articul = "";
                Name = "";
                Stocks = 0;
                Multiplicity = 0;
                Marketplace = EMarketplaces.None;
                Barcodes.Clear();
                IsDisplay = false;
            }
        }

        /// <summary>
        /// Seller articul
        /// </summary>
        [ObservableProperty]
        private string _articul = "";

        /// <summary>
        /// Product card name determines by seller in marketplace
        /// </summary>
        [ObservableProperty]
        private string _name = "";

        /// <summary>
        /// Current product card stocks
        /// </summary>
        [ObservableProperty]
        private int _stocks = 0;

        /// <summary>
        /// Product card creation data from marketplace
        /// </summary>
        [ObservableProperty]
        private DateTime _creationDate = new();

        /// <summary>
        /// Flag for change stocks in other cards in onion. If false update only this card
        /// </summary>
        [ObservableProperty]
        private bool _isSynch = true;

        /// <summary>
        /// Count of products for update other cards in onion.
        /// Example: 2 cards of 1 product, different by count.
        /// This count should be indicated in the multiplicity
        /// </summary>
        [ObservableProperty]
        private int _multiplicity = 0;

        /// <summary>
        /// Marketplace where product card from
        /// </summary>
        [ObservableProperty]
        private EMarketplaces _marketplace = EMarketplaces.None;

        /// <summary>
        /// Barcodes of product card
        /// </summary>
        public ObservableCollection<string> Barcodes { get; set; } = new();

        /// <summary>
        /// Display this detail or collapse
        /// </summary>
        [ObservableProperty]
        private bool _isDisplay = false;
    }
}
