using System;

using BF.Appliance.ViewModels;

using Windows.UI.Xaml.Controls;

namespace BF.Appliance.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
