// Views/PatientViews/PaymentWindow.xaml.cs
using BDAS2_SEM.ViewModel;
using System.Windows;

namespace BDAS2_SEM.View.PatientViews
{
    public partial class PaymentWindow : Window
    {
        public PaymentWindow(decimal amount)
        {
            InitializeComponent();
            var viewModel = new PaymentWindowVM(amount);
            this.DataContext = viewModel;

            // Підписка на подію закриття вікна
            viewModel.CloseWindowEvent += ViewModel_CloseWindowEvent;
        }

        private void ViewModel_CloseWindowEvent(object sender, bool? e)
        {
            this.DialogResult = e;
            this.Close();
        }
    }
}