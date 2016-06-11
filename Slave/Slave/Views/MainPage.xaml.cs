using System;
using Slave.ViewModels;
using Slave.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace Slave.Views
{
    public sealed partial class MainPage : Page
    {
        MainPageViewModel VM { get; set; } = new MainPageViewModel();
        public MainPage()
        {
            InitializeComponent();  
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }


    }
}
