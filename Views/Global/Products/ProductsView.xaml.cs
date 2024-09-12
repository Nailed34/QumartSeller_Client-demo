/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using System.Windows.Controls;
using System.Windows.Input;

namespace ClientWPF.Views.Global.Products
{
    public partial class ProductsView : Page
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        private void textBoxSearch_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (textBoxSearch.Text == "Поиск")
                textBoxSearch.Text = "";
        }

        private void textBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Keyboard.ClearFocus();
        }
    }
}
