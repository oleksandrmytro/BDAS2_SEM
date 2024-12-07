using BDAS2_SEM.ViewModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace BDAS2_SEM.View.AdminViews
{
    public partial class AnalyzeView : UserControl
    {
        public AnalyzeView(AnalyzeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+"); // Разрешаем только цифры
            return !regex.IsMatch(text);
        }
    }
}