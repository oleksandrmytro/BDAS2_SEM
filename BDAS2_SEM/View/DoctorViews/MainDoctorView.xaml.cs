// View/DoctorViews/MainDoctorView.xaml.cs
using System.Windows.Controls;
using BDAS2_SEM.Model;
using BDAS2_SEM.ViewModel;

namespace BDAS2_SEM.View.DoctorViews
{
    public partial class MainDoctorView : UserControl
    {
        public MainDoctorView(MainDoctorVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
