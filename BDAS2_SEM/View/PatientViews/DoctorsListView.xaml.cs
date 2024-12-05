using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BDAS2_SEM.ViewModel;

namespace BDAS2_SEM.View.PatientViews
{
    /// <summary>
    /// Interaction logic for DoctorsListView.xaml
    /// </summary>
    public partial class DoctorsListView : Window
    {
        public DoctorsListView()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<DoctorsListVM>();
        }
    }
}