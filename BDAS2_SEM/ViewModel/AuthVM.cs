// ViewModel/AuthVM.cs
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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

        private readonly IAuthenticationService _authenticationService;
        private readonly IWindowService _windowService;

        public AuthVM(IAuthenticationService authenticationService, IWindowService windowService)
        {
            _authenticationService = authenticationService;
            _windowService = windowService;
            IsRegistering = false; // Start with login view
            ToggleRegisterCommand = new RelayCommand((o) => ToggleRegister(o));
            SubmitCommand = new RelayCommand(async (o) => await SubmitAsync(), CanSubmit);
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

        // Commands
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

        private async Task SubmitAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (!IsValidEmail(Email))
                {
                    ErrorMessage = "Invalid email format.";
                    return;
                }

                if (IsRegistering)
                {
                    bool success = await _authenticationService.RegisterUserAsync(Email, Heslo);
                    if (success)
                    {
                        ErrorMessage = "Registration successful!";
                        ClearFields();
                        IsRegistering = false;
                    }
                    else
                    {
                        var existingUser = await _authenticationService.CheckUserExistsAsync(Email);
                        if (existingUser)
                        {
                            ErrorMessage = "A user with this email already exists.";
                        }
                        else
                        {
                            ErrorMessage = "An error occurred during registration. Please try again.";
                        }
                    }
                }
                else
                {
                    var user = await _authenticationService.LoginAsync(Email, Heslo);
                    if (user != null)
                    {
                        PACIENT pacient = await _authenticationService.GetPatientByUserId(user.Id);
                        ErrorMessage = "Login successful!";

                        if (user.RoleUzivatel == Role.ADMIN)
                        {
                            _windowService.OpenAdminWindow();
                        }
                        else if (user.RoleUzivatel == Role.PACIENT)
                        {
                            bool isPatientDataComplete = await _authenticationService.IsPatientDataComplete(user.Id);

                            if (!isPatientDataComplete)
                            {
                                _windowService.OpenNewPatientWindow(user, async (bool isSaved) =>
                                {
                                    if (isSaved)
                                    {
                                        _windowService.OpenPatientWindow(pacient);
                                        CloseAuthWindow();
                                    }
                                    else
                                    {
                                        _windowService.OpenAuthWindow();
                                    }
                                });
                            }
                            else
                            {
                                _windowService.OpenPatientWindow(pacient);
                            }
                        }
                        else
                        {
                            _windowService.OpenPatientWindow(pacient);
                        }

                        CloseAuthWindow();
                    }
                    else
                    {
                        ErrorMessage = "Invalid email or password.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseAuthWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is AuthWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@");
        }

        private void ClearFields()
        {
            Email = string.Empty;
            Heslo = string.Empty;
            ConfirmPassword = string.Empty;
            ErrorMessage = string.Empty;
        }

        // Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Invoke RaiseCanExecuteChanged when properties change
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