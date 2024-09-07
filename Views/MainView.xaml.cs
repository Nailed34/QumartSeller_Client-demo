/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Services;
using System.Windows;

namespace ClientWPF.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            NavigationService.RegisterNavigationFrame("FrameMain", FrameMain);
        }
    }
}