/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ClientWPF.ViewModels.Global.Products
{
    internal partial class ProductsViewModel : ObservableObject
    {
        const int MAX_CARDS_IN_PAGE = ProductsModel.MAX_CARDS_IN_PAGE;
        private readonly ProductsModel _model;

        public ProductsViewModel()
        {
            _model = new();
            _model.PropertyChanged += BindModelChanges;

            _model.InGetProductsInfo(1, MAX_CARDS_IN_PAGE);
        }    

        private void BindModelChanges(object? obj, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == null)
                return;

            if (args.PropertyName == nameof(_model.ProductsCount))
            {
                ProductsCount = _model.ProductsCount;
                return;
            }

            OnPropertyChanged(args.PropertyName);
        }

        [RelayCommand]
        private void NextPage()
        {
            CurrentPageNumber++;
        }

        [RelayCommand]
        private void BackPage()
        {
            CurrentPageNumber--;
        }

        public ReadOnlyObservableCollection<ProductCard> ProductCards => _model.Products;

        public int ProductsCount
        {
            get => _productsCount;
            set
            {
                if (value == _productsCount) return;

                _productsCount = value;
                OnPropertyChanged(nameof(ProductsCount));

                IsEmptyCards = value == 0;
                CanNavigatePage = value > MAX_CARDS_IN_PAGE;
                ProductsPagesCount = (int)Math.Ceiling((double)value / MAX_CARDS_IN_PAGE);
            }
        }
        private int _productsCount = 0;

        public int ProductsPagesCount
        {
            get => _productsPagesCount;
            set
            {
                if (value == _productsPagesCount) return;

                _productsPagesCount = value;
                OnPropertyChanged(nameof(ProductsPagesCount));

                CurrentPageNumber = value == 0 ? 0 : Math.Clamp(CurrentPageNumber, 1, ProductsPagesCount);
            }
        }
        private int _productsPagesCount = 0;

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set
            {
                if (value == _currentPageNumber) return;
                
                _currentPageNumber = Math.Clamp(value, 1, ProductsPagesCount);
                OnPropertyChanged(nameof(CurrentPageNumber));

                IsBackButtonEnabled = IsEmptyCards ? false : _currentPageNumber != 1;
                IsNextButtonEnabled = IsEmptyCards ? false : _currentPageNumber != ProductsPagesCount;

                _model.InGetProductsInfo(MAX_CARDS_IN_PAGE * (_currentPageNumber - 1) + 1, MAX_CARDS_IN_PAGE * _currentPageNumber);
            }
        }
        private int _currentPageNumber = 0;

        [ObservableProperty]
        private bool _isEmptyCards = true;

        [ObservableProperty]
        private bool _isBackButtonEnabled = false;

        [ObservableProperty]
        private bool _isNextButtonEnabled = false;

        [ObservableProperty]
        private bool _canNavigatePage = false;
    }
}
