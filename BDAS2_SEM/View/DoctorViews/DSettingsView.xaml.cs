using BDAS2_SEM.ViewModel;
using System.Windows.Controls;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;

namespace BDAS2_SEM.View.DoctorViews
{
    /// <summary>
    /// Логика взаимодействия для DSettingsView.xaml
    /// </summary>
    public partial class DSettingsView : UserControl
    {
        public DSettingsView(DSettingsVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}