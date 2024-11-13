using System.Windows;
using System.Windows.Controls;

namespace BDAS2_SEM.Controls
{
    public partial class PasswordBoxWithShow : UserControl
    {
        private bool _isUpdating = false;

        public PasswordBoxWithShow()
        {
            InitializeComponent();
            HiddenPasswordBox.PasswordChanged += HiddenPasswordBox_PasswordChanged;
            VisiblePasswordBox.TextChanged += VisiblePasswordBox_TextChanged;
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxWithShow), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PasswordBoxWithShow)d;
            control.UpdatePasswordBoxes();
        }

        private void UpdatePasswordBoxes()
        {
            if (_isUpdating)
                return;

            _isUpdating = true;
            VisiblePasswordBox.Text = Password;
            HiddenPasswordBox.Password = Password;
            _isUpdating = false;
        }

        private void HiddenPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdating)
                return;

            _isUpdating = true;
            Password = HiddenPasswordBox.Password;
            _isUpdating = false;
        }

        private void VisiblePasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating)
                return;

            _isUpdating = true;
            Password = VisiblePasswordBox.Text;
            _isUpdating = false;
        }

        public static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.Register("IsPasswordVisible", typeof(bool), typeof(PasswordBoxWithShow), new PropertyMetadata(false));

        public bool IsPasswordVisible
        {
            get => (bool)GetValue(IsPasswordVisibleProperty);
            set => SetValue(IsPasswordVisibleProperty, value);
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }
    }
}
