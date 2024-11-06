using BDAS2_SEM.Commands;
using BDAS2_SEM.DataAccess;
using BDAS2_SEM.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class AuthVM : INotifyPropertyChanged
    {
        private string email;
        private string heslo;
        private string confirmPassword;
        private string errorMessage;
        private bool isRegistering;

        private readonly OracleDataAccess dataAccess;

        public AuthVM()
        {
            dataAccess = new OracleDataAccess();
            IsRegistering = false; // Починаємо з виду входу
            ToggleRegisterCommand = new RelayCommand(ToggleRegister);
            SubmitCommand = new RelayCommand(Submit, CanSubmit);
        }

        public string Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Heslo
        {
            get => heslo;
            set
            {
                if (heslo != value)
                {
                    heslo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                if (confirmPassword != value)
                {
                    confirmPassword = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRegistering
        {
            get => isRegistering;
            set
            {
                if (isRegistering != value)
                {
                    isRegistering = value;
                    OnPropertyChanged();
                }
            }
        }

        // Команди
        public RelayCommand ToggleRegisterCommand { get; }
        public RelayCommand SubmitCommand { get; }

        private void ToggleRegister(object parameter)
        {
            IsRegistering = !IsRegistering;
            ClearFields();
        }

        private bool CanSubmit(object parameter)
        {
            if (IsRegistering)
            {
                return !string.IsNullOrEmpty(Email) &&
                       !string.IsNullOrEmpty(Heslo) &&
                       !string.IsNullOrEmpty(ConfirmPassword) &&
                       Heslo == ConfirmPassword;
            }
            else
            {
                return !string.IsNullOrEmpty(Email) &&
                       !string.IsNullOrEmpty(Heslo);
            }
        }

        private void Submit(object parameter)
        {
            ErrorMessage = string.Empty;

            if (IsRegistering)
            {
                Register();
            }
            else
            {
                Login();
            }
        }

        private void Register()
        {
            try
            {
                // Створюємо об'єкт UZIVATEL_DATA
                UZIVATEL_DATA newUser = new UZIVATEL_DATA
                {
                    Email = this.Email,
                    Heslo = this.Heslo // Зберігаємо пароль як є
                };

                // Перевіряємо, чи користувач з таким email вже існує
                string checkQuery = "SELECT COUNT(*) FROM USERS WHERE EMAIL = :email";
                OracleParameter emailParam = new OracleParameter("email", newUser.Email);

                DataTable existingUser = dataAccess.ExecuteQuery(checkQuery, emailParam);

                if (existingUser.Rows.Count > 0 && Convert.ToInt32(existingUser.Rows[0][0]) > 0)
                {
                    ErrorMessage = "Користувач з таким email вже існує.";
                    return;
                }

                // Вставляємо нового користувача у базу даних
                int rowsInserted = dataAccess.InsertUzivatel(newUser);

                if (rowsInserted > 0)
                {
                    ErrorMessage = "Реєстрація успішна!";
                    ClearFields();
                    IsRegistering = false;
                }
                else
                {
                    ErrorMessage = "Помилка при реєстрації.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Помилка при реєстрації: " + ex.Message;
            }
        }

        private void Login()
        {
            try
            {
                string query = "SELECT * FROM UZIVATEL_DATA WHERE EMAIL = :email AND PASSWORD = :password";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("email", this.Email),
                    new OracleParameter("password", this.Heslo) // Пароль зберігається як є
                };

                DataTable userTable = dataAccess.ExecuteQuery(query, parameters);

                if (userTable.Rows.Count > 0)
                {
                    ErrorMessage = "Вхід успішний!";
                    // Додаткова логіка після успішного входу
                }
                else
                {
                    ErrorMessage = "Невірний email або пароль.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Помилка при спробі входу: " + ex.Message;
            }
        }

        private void ClearFields()
        {
            Email = string.Empty;
            Heslo = string.Empty;
            ConfirmPassword = string.Empty;
            ErrorMessage = string.Empty;
        }

        // Реалізація INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Викликаємо RaiseCanExecuteChanged при зміні властивостей
            if (propertyName == nameof(Email) ||
                propertyName == nameof(Heslo) ||
                propertyName == nameof(ConfirmPassword) ||
                propertyName == nameof(IsRegistering))
            {
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
