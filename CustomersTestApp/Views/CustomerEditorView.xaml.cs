using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomersTestApp.Views;

public partial class CustomerEditorView : UserControl
{
    public CustomerEditorView()
    {
        InitializeComponent();
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    private void NumericOnly(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+"); // Regex that matches disallowed text
        e.Handled = regex.IsMatch(e.Text);
    }
}