using System;
using System.Collections.Generic;
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
using BDAS2_SEM.ViewModel;

namespace BDAS2_SEM.View.AdminViews
{
    /// <summary>
    /// Interaction logic for SystemCatalogView.xaml
    /// </summary>
    public partial class SystemCatalogView : UserControl
    {
        public SystemCatalogView(SystemCatalogVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
