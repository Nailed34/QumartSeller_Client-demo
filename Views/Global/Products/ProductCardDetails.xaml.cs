/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientServerConnection;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ClientWPF.Views.Global.Products
{
    /// <summary>
    /// Логика взаимодействия для ProductCardDetails.xaml
    /// </summary>
    public partial class ProductCardDetails : UserControl
    {
        static ProductCardDetails()
        {
            DetailArticulProperty = DependencyProperty.Register("DetailArticul", typeof(string), typeof(ProductCardDetails));
            DetailNameProperty = DependencyProperty.Register("DetailName", typeof(string), typeof(ProductCardDetails));
            DetailStocksProperty = DependencyProperty.Register("DetailStocks", typeof(int), typeof(ProductCardDetails));
            DetailCreationDateProperty = DependencyProperty.Register("DetailCreationDate", typeof(DateTime), typeof(ProductCardDetails));
            DetailIsSynchProperty = DependencyProperty.Register("DetailIsSynch", typeof(bool), typeof(ProductCardDetails));
            DetailMultiplicityProperty = DependencyProperty.Register("DetailMultiplicity", typeof(int), typeof(ProductCardDetails));
            DetailMarketplaceProperty = DependencyProperty.Register("DetailMarketplace", typeof(EMarketplaces), typeof(ProductCardDetails));
            DetailBarcodesProperty = DependencyProperty.Register("DetailBarcodes", typeof(ObservableCollection<string>), typeof(ProductCardDetails), new PropertyMetadata(OnDetailBarcodesPropertyChanged));
            DetailIsDisplayProperty = DependencyProperty.Register("DetailIsDisplay", typeof(bool), typeof(ProductCardDetails));
        }

        public ProductCardDetails()
        {
            InitializeComponent();
        }

        private static void OnDetailBarcodesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentCard = (ProductCardDetails)d;
            var newValue = (ObservableCollection<string>)e.NewValue;
            newValue.CollectionChanged += currentCard.OnDetailBarcodesChanged;
        }

        private void OnDetailBarcodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        public static DependencyProperty DetailArticulProperty;
        public static DependencyProperty DetailNameProperty;
        public static DependencyProperty DetailStocksProperty;
        public static DependencyProperty DetailCreationDateProperty;
        public static DependencyProperty DetailIsSynchProperty;
        public static DependencyProperty DetailMultiplicityProperty;
        public static DependencyProperty DetailMarketplaceProperty;
        public static DependencyProperty DetailBarcodesProperty;
        public static DependencyProperty DetailIsDisplayProperty;

        public string DetailArticul
        {
            get { return (string)GetValue(DetailArticulProperty); }
            set { SetValue(DetailArticulProperty, value); }
        }
        public string DetailName
        {
            get { return (string)GetValue(DetailNameProperty); }
            set { SetValue(DetailNameProperty, value); }
        }
        public int DetailStocks
        {
            get { return (int)GetValue(DetailStocksProperty); }
            set { SetValue(DetailStocksProperty, value); }
        }
        public DateTime DetailCreationDate
        {
            get { return (DateTime)GetValue(DetailCreationDateProperty); }
            set { SetValue(DetailCreationDateProperty, value); }
        }
        public bool DetailIsSynch
        {
            get { return (bool)GetValue(DetailIsSynchProperty); }
            set { SetValue(DetailIsSynchProperty, value); }
        }
        public int DetailMultiplicity
        {
            get { return (int)GetValue(DetailMultiplicityProperty); }
            set { SetValue(DetailMultiplicityProperty, value); }
        }
        public EMarketplaces DetailMarketplace
        {
            get { return (EMarketplaces)GetValue(DetailMarketplaceProperty); }
            set { SetValue(DetailMarketplaceProperty, value); }
        }
        public ObservableCollection<string> DetailBarcodes
        {
            get { return (ObservableCollection<string>)GetValue(DetailBarcodesProperty); }
            set { SetValue(DetailBarcodesProperty, value); }
        }
        public bool DetailIsDisplay
        {
            get { return (bool)GetValue(DetailIsDisplayProperty); }
            set { SetValue(DetailIsDisplayProperty, value); }
        }
    }
}
