/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientServerConnection;
using ClientWPF.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClientWPF.Views.Global.Products
{
    /// <summary>
    /// Логика взаимодействия для ProductCard.xaml
    /// </summary>
    public partial class ProductCard : UserControl
    {
        static ProductCard()
        {
            CardIdProperty = DependencyProperty.Register("CardId", typeof(string), typeof(ProductCard));
            CardNameProperty = DependencyProperty.Register("CardName", typeof(string), typeof(ProductCard));
            CardPhotoProperty = DependencyProperty.Register("CardPhoto", typeof(string), typeof(ProductCard), new PropertyMetadata(OnCardPhotoPropertyChanged));
            CardStocksProperty = DependencyProperty.Register("CardStocks", typeof(int), typeof(ProductCard));
            CardArticulsProperty = DependencyProperty.Register("CardArticuls", typeof(ObservableCollection<string>), typeof(ProductCard), new PropertyMetadata(OnCardArticulsPropertyChanged));
            CardBarcodesProperty = DependencyProperty.Register("CardBarcodes", typeof(ObservableCollection<string>), typeof(ProductCard), new PropertyMetadata(OnCardBarcodesPropertyChanged));
            CardMarketplacesProperty = DependencyProperty.Register("CardMarketplaces", typeof(ObservableCollection<EMarketplaces>), typeof(ProductCard), new PropertyMetadata(OnCardMarketplacesPropertyChanged));
            CardDetailsProperty = DependencyProperty.Register("CardDetails", typeof(ObservableCollection<ProductInfoCard>), typeof(ProductCard), new PropertyMetadata(OnCardDetailsPropertyChanged));
            CardIsDisplayProperty = DependencyProperty.Register("CardIsDisplay", typeof(bool), typeof(ProductCard));

            SelectedArticulProperty = DependencyProperty.Register("SelectedArticul", typeof(string), typeof(ProductCard));
            SelectedBarcodeProperty = DependencyProperty.Register("SelectedBarcode", typeof(string), typeof(ProductCard));           

            AdditiveArticulsCountProperty = DependencyProperty.Register("AdditiveArticulsCount", typeof(int), typeof(ProductCard));
            IsDisplayArticulsCountProperty = DependencyProperty.Register("IsDisplayArticulsCount", typeof(bool), typeof(ProductCard));
            AdditiveBarcodesCountProperty = DependencyProperty.Register("AdditiveBarcodesCount", typeof(int), typeof(ProductCard));
            IsDisplayBarcodesCountProperty = DependencyProperty.Register("IsDisplayBarcodesCount", typeof(bool), typeof(ProductCard));

            IsDisplayMarketplaceOzonProperty = DependencyProperty.Register("IsDisplayMarketplaceOzon", typeof(bool), typeof(ProductCard));
            IsDisplayMarketplaceWBProperty = DependencyProperty.Register("IsDisplayMarketplaceWB", typeof(bool), typeof(ProductCard));
            IsDisplayMarketplaceYandexProperty = DependencyProperty.Register("IsDisplayMarketplaceYandex", typeof(bool), typeof(ProductCard));

            ProductDetailsProperty = DependencyProperty.Register("ProductDetauls", typeof(ObservableCollection<Models.ProductCardDetails>), typeof(ProductCard));
            IsDisplayDetailsProperty = DependencyProperty.Register("IsDisplayDetails", typeof(bool), typeof(ProductCard));
        }

        private readonly PhotoCacheService _photoCacheService;

        public ProductCard()
        {
            InitializeComponent();

            _photoCacheService = new();
            _photoCacheService.PhotoLoadedEvent += OnPhotoLoaded;

            // Create first card detail
            ProductDetails = new() { new Models.ProductCardDetails() };
        }

        private void SwapDetailsDisplaying(object sender, RoutedEventArgs e)
        {
            IsDisplayDetails = !IsDisplayDetails;
        }

        private static void OnCardArticulsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentCard = (ProductCard)d;
            var newValue = (ObservableCollection<string>)e.NewValue;
            newValue.CollectionChanged += currentCard.OnCardArticulsChanged;
        }
        private static void OnCardBarcodesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentCard = (ProductCard)d;
            var newValue = (ObservableCollection<string>)e.NewValue;
            newValue.CollectionChanged += currentCard.OnCardBarcodesChanged;
        }
        private static void OnCardMarketplacesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentCard = (ProductCard)d;
            var newValue = (ObservableCollection<EMarketplaces>)e.NewValue;
            newValue.CollectionChanged += currentCard.OnCardMarketplacesChanged;
        }
        private static void OnCardDetailsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentCard = (ProductCard)d;
            var newValue = (ObservableCollection<ProductInfoCard>)e.NewValue;
            newValue.CollectionChanged += currentCard.OnCardDetailsChanged;
        }
        private static void OnCardPhotoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldPhoto = (string)e.OldValue;
            var newPhoto = (string)e.NewValue;
            if (oldPhoto != newPhoto)
                ((ProductCard)d).OnCardPhotoChanged((string)e.NewValue);       
        }
        private void OnCardArticulsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedArticul = CardArticuls.Count > 0 ? CardArticuls[0] : "";
            AdditiveArticulsCount = Math.Max(CardArticuls.Count - 1, 0);
            IsDisplayArticulsCount = AdditiveArticulsCount > 0;
        }
        private void OnCardBarcodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedBarcode = CardBarcodes.Count > 0 ? CardBarcodes[0] : "Отсутствует";
            AdditiveBarcodesCount = Math.Max(CardBarcodes.Count - 1, 0);
            IsDisplayBarcodesCount = AdditiveBarcodesCount > 0;
        }
        private void OnCardMarketplacesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            IsDisplayMarketplaceOzon = false;
            IsDisplayMarketplaceWB = false;
            IsDisplayMarketplaceYandex = false;

            foreach (var marketplace in CardMarketplaces)
            {
                switch (marketplace)
                {
                    case EMarketplaces.Ozon:
                        IsDisplayMarketplaceOzon = true;
                        break;
                    case EMarketplaces.Wildberries:
                        IsDisplayMarketplaceWB = true;
                        break;
                    case EMarketplaces.YandexMarket:
                        IsDisplayMarketplaceYandex = true;
                        break;
                }
            }
        }
        private void OnCardDetailsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                // Setup all details to default
                case NotifyCollectionChangedAction.Reset:
                    foreach (var detail in ProductDetails)
                        detail.ChangeSource(null);
                    break;

                // Setup existed detail or create new
                case NotifyCollectionChangedAction.Add:
                    // Create new
                    if (CardDetails.Count > ProductDetails.Count)
                        ProductDetails.Add(new Models.ProductCardDetails());

                    // Update detail
                    ProductDetails[CardDetails.Count - 1].ChangeSource(CardDetails.Last());
                    break;
            }
            IsDisplayDetails = false;
        }
        private void OnCardPhotoChanged(string newValue)
        {
            borderPhoto.IsEnabled = false;

            if (newValue != "")
                _photoCacheService.LoadPhoto(CardId, CardPhoto);
        }

        private void OnPhotoLoaded(BitmapImage? newPhoto)
        {
            Dispatcher.Invoke(() =>
            {
                if (newPhoto != null)
                {
                    imagePhoto.Source = newPhoto;
                    borderPhoto.IsEnabled = true;
                }
            });          
        }

        // Dependency properties
        public static DependencyProperty CardIdProperty;
        public static DependencyProperty CardNameProperty;
        public static DependencyProperty CardPhotoProperty;
        public static DependencyProperty CardStocksProperty;
        public static DependencyProperty CardArticulsProperty;
        public static DependencyProperty CardBarcodesProperty;
        public static DependencyProperty CardMarketplacesProperty;
        public static DependencyProperty CardDetailsProperty;
        public static DependencyProperty CardIsDisplayProperty;

        public static DependencyProperty SelectedArticulProperty;
        public static DependencyProperty SelectedBarcodeProperty;      
        public static DependencyProperty AdditiveArticulsCountProperty;
        public static DependencyProperty IsDisplayArticulsCountProperty;
        public static DependencyProperty AdditiveBarcodesCountProperty;
        public static DependencyProperty IsDisplayBarcodesCountProperty;

        public static DependencyProperty IsDisplayMarketplaceOzonProperty;
        public static DependencyProperty IsDisplayMarketplaceWBProperty;
        public static DependencyProperty IsDisplayMarketplaceYandexProperty;

        public static DependencyProperty ProductDetailsProperty;
        public static DependencyProperty IsDisplayDetailsProperty;

        // Main props
        public string CardId
        {
            get { return (string)GetValue(CardIdProperty); }
            set { SetValue(CardIdProperty, value); }
        }
        public string CardName
        {
            get { return (string)GetValue(CardNameProperty); }
            set { SetValue(CardNameProperty, value); }
        }
        public string CardPhoto
        {
            get { return (string)GetValue(CardPhotoProperty); }
            set { SetValue(CardPhotoProperty, value); }
        }
        public int CardStocks
        {
            get { return (int)GetValue(CardStocksProperty); }
            set { SetValue(CardStocksProperty, value); }
        }
        public ObservableCollection<string> CardArticuls
        {
            get { return (ObservableCollection<string>)GetValue(CardArticulsProperty); }
            set { SetValue(CardArticulsProperty, value); }
        }
        public ObservableCollection<string> CardBarcodes
        {
            get { return (ObservableCollection<string>)GetValue(CardBarcodesProperty); }
            set { SetValue(CardBarcodesProperty, value); }
        }
        public ObservableCollection<EMarketplaces> CardMarketplaces
        {
            get { return (ObservableCollection<EMarketplaces>)GetValue(CardMarketplacesProperty); }
            set { SetValue(CardMarketplacesProperty, value); }
        }
        public ObservableCollection<ProductInfoCard> CardDetails
        {
            get { return (ObservableCollection<ProductInfoCard>)GetValue(CardDetailsProperty); }
            set { SetValue(CardDetailsProperty, value); }
        }
        public bool CardIsDisplay
        {
            get { return (bool)GetValue(CardIsDisplayProperty); }
            set { SetValue(CardIsDisplayProperty, value); }
        }

        // Additive visual props
        public string SelectedArticul
        {
            get { return (string)GetValue(SelectedArticulProperty); }
            set { SetValue(SelectedArticulProperty, value); }
        }      
        public string SelectedBarcode
        {
            get { return (string)GetValue(SelectedBarcodeProperty); }
            set { SetValue(SelectedBarcodeProperty, value); }
        }    
        public int AdditiveArticulsCount
        {
            get { return (int)GetValue(AdditiveArticulsCountProperty); }
            set { SetValue(AdditiveArticulsCountProperty, value); }
        }
        public bool IsDisplayArticulsCount
        {
            get { return (bool)GetValue(IsDisplayArticulsCountProperty); }
            set { SetValue(IsDisplayArticulsCountProperty, value); }
        }
        public int AdditiveBarcodesCount
        {
            get { return (int)GetValue(AdditiveBarcodesCountProperty); }
            set { SetValue(AdditiveBarcodesCountProperty, value); }
        }
        public bool IsDisplayBarcodesCount
        {
            get { return (bool)GetValue(IsDisplayBarcodesCountProperty); }
            set { SetValue(IsDisplayBarcodesCountProperty, value); }
        }

        public bool IsDisplayMarketplaceOzon
        {
            get { return (bool)GetValue(IsDisplayMarketplaceOzonProperty); }
            set { SetValue(IsDisplayMarketplaceOzonProperty, value); }
        }
        public bool IsDisplayMarketplaceWB
        {
            get { return (bool)GetValue(IsDisplayMarketplaceWBProperty); }
            set { SetValue(IsDisplayMarketplaceWBProperty, value); }
        }
        public bool IsDisplayMarketplaceYandex
        {
            get { return (bool)GetValue(IsDisplayMarketplaceYandexProperty); }
            set { SetValue(IsDisplayMarketplaceYandexProperty, value); }
        }
        public ObservableCollection<Models.ProductCardDetails> ProductDetails
        {
            get { return (ObservableCollection<Models.ProductCardDetails>)GetValue(ProductDetailsProperty); }
            set { SetValue(ProductDetailsProperty, value); }
        }
        public bool IsDisplayDetails
        {
            get { return (bool)GetValue(IsDisplayDetailsProperty); }
            set { SetValue(IsDisplayDetailsProperty, value); }
        }
    }
}
