using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CustomersTestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<ViewModels.MainViewModel>();
        }
    }
}