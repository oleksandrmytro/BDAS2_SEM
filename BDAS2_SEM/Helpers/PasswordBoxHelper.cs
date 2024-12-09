using System.Windows;
using System.Windows.Controls;

namespace BDAS2_SEM.Helpers
{
    // Statická třída PasswordBoxHelper umožňuje obousměrné vázání hodnoty PasswordBox na vlastnost typu string.
    public static class PasswordBoxHelper
    {
        // Připojená vlastnost BoundPassword pro obousměrné vázání hesla.
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        // Připojená vlastnost BindPassword pro určení, zda je heslo vázáno.
        public static readonly DependencyProperty BindPassword =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        // Interní vlastnost pro sledování aktualizace hesla, aby se zabránilo nekonečné smyčce.
        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        // Získává hodnotu připojené vlastnosti BoundPassword.
        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        // Nastavuje hodnotu připojené vlastnosti BoundPassword.
        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            dp.SetValue(BoundPassword, value);
        }

        // Získává hodnotu připojené vlastnosti BindPassword.
        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        // Nastavuje hodnotu připojené vlastnosti BindPassword.
        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        // Získává hodnotu interní vlastnosti UpdatingPassword.
        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        // Nastavuje hodnotu interní vlastnosti UpdatingPassword.
        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }

        // Handler pro změnu vlastnosti BoundPassword.
        private static void OnBoundPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && GetBindPassword(passwordBox))
            {
                passwordBox.PasswordChanged -= HandlePasswordChanged;

                // Nastaví nové heslo pouze v případě, že se neaktualizuje aktuální hodnota.
                string newPassword = (string)e.NewValue ?? string.Empty;

                if (!GetUpdatingPassword(passwordBox))
                {
                    passwordBox.Password = newPassword;
                }

                passwordBox.PasswordChanged += HandlePasswordChanged;
            }
        }

        // Handler pro změnu vlastnosti BindPassword.
        private static void OnBindPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                bool wasBound = (bool)(e.OldValue);
                bool needToBind = (bool)(e.NewValue);

                // Odpojí nebo připojí událost PasswordChanged podle nové hodnoty.
                if (wasBound)
                {
                    passwordBox.PasswordChanged -= HandlePasswordChanged;
                }

                if (needToBind)
                {
                    passwordBox.PasswordChanged += HandlePasswordChanged;
                }
            }
        }

        // Handler pro událost PasswordChanged, synchronizuje heslo s vlastností BoundPassword.
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetUpdatingPassword(passwordBox, true);
                SetBoundPassword(passwordBox, passwordBox.Password);
                SetUpdatingPassword(passwordBox, false);
            }
        }
    }
}
