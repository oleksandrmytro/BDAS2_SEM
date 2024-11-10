// ViewModel/AuthVM.cs
using BDAS2_SEM.Commands;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        public AuthVM(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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
                bool success = await _authenticationService.LoginAsync(Email, Heslo);
                if (success)
                {
                    ErrorMessage = "Login successful!";
                    // Additional logic after successful login, e.g., opening the main window
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
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