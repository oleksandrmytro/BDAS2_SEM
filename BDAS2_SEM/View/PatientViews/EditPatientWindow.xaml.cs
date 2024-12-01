using BDAS2_SEM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BDAS2_SEM.View.PatientViews
{
    /// <summary>
    /// Логика взаимодействия для EditPatientWindow.xaml
    /// </summary>
    public partial class EditPatientWindow : Window
    {
        public EditPatientWindow()
        {
            InitializeComponent();
            this.Closing += EditPatientWindow_Closing;
        }

        private void EditPatientWindow_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is EditPatientVM viewModel)
            {
                viewModel.InvokeOnClosed(false);
            }
        }
    }
}
