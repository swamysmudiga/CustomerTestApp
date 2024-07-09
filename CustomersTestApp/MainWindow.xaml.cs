using CustomersTestApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CustomersTestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
        }
    }
}
