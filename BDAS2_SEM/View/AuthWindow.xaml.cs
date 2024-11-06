using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BDAS2_SEM.ViewModel;

namespace BDAS2_SEM.View
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AuthVM viewModel)
            {
                if (sender is PasswordBox passwordBox)
                {
                    string password = passwordBox.Password;
                    string tag = passwordBox.Tag as string;

                    switch (tag)
                    {
                        case "LoginPassword":
                        case "RegisterPassword":
                            viewModel.Heslo = password;
                            break;
                        case "ConfirmPassword":
                            viewModel.ConfirmPassword = password;
                            break;
                    }
                }
            }
        }

    }
}
