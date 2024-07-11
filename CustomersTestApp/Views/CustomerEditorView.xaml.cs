using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomersTestApp.Views
{
    public partial class CustomerEditorView : UserControl
    {
        public CustomerEditorView()
        {
            InitializeComponent();
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+"); // Regex that matches non-numeric text
            return !regex.IsMatch(text);
        }
    }
}
