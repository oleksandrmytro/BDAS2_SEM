using BDAS2_SEM.ViewModel;
using System;
using System.Windows;

namespace BDAS2_SEM.View.PatientViews
{
    /// <summary>
    /// Interaction logic for PatientsWindow.xaml
    /// </summary>
    public partial class PatientsWindow : Window
    {
        public PatientsWindow(PatientsVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}