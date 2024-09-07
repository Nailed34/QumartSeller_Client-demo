/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using ClientWPF.Services;
using System.Windows.Controls;

namespace ClientWPF.Views.Global
{
    public partial class GlobalView : Page
    {
        public GlobalView()
        {
            InitializeComponent();
            Services.NavigationService.RegisterNavigationFrame("FrameGlobal", FrameGlobal);
        }
    }
}
