using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using CustomersTestApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CustomersTestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Discount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "0";
                }

                if (int.TryParse(textBox.Text, out int discount))
                {
                    if (discount < 0 || discount > 30)
                    {
                        MessageBox.Show("Discount should be between 0 to 30", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        textBox.Text = "0"; // Reset to a valid value
                    }
                }
            }
        }
    }
}
