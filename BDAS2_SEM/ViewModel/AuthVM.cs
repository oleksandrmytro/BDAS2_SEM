// ViewModel/AuthVM.cs
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces; // Add this using directive
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
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
        private readonly IZamestnanecRepository _zamestnanecRepository; // Add this field

        public AuthVM(IAuthenticationService authenticationService, IWindowService windowService, IZamestnanecRepository zamestnanecRepository)
        {
            _authenticationService = authenticationService;
            _windowService = windowService;
            _zamestnanecRepository = zamestnanecRepository; // Initialize the repository
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
                        if (user.RoleId == 1)
                        {
                            ErrorMessage = "Your account has not been approved yet. Please wait for approval.";
                            return;
                        }

                        PACIENT pacient = await _authenticationService.GetPatientByUserId(user.Id);
                        ErrorMessage = "Login successful!";

                        if (user.RoleId == 4)
                        {
                            _windowService.OpenAdminWindow();
                        }
                        else if (user.RoleId == 2)
                        {
                            bool isPatientDataComplete = await _authenticationService.IsPatientDataComplete(user.Id);

                            if (!isPatientDataComplete)
                            {
                                _windowService.OpenNewPatientWindow(user, async (bool isSaved) =>
                                {
                                    if (isSaved)
                                    {
                                        pacient = await _authenticationService.GetPatientByUserId(user.Id);
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
                            if (user.zamestnanecId.HasValue)
                            {
                                var zamestnanec = await _zamestnanecRepository.GetZamestnanecById(user.zamestnanecId.Value);
                                if (zamestnanec != null)
                                {
                                    if (zamestnanec.PoziceId == 1 || zamestnanec.PoziceId == 4) 
                                    {
                                        _windowService.OpenDoctorWindow(zamestnanec);
                                    }
                                    else
                                    {
                                        // Handle other employee positions if necessary
                                        // For example, open a default employee window
                                        //_windowService.OpenEmployeeWindow(user);
                                    }
                                }
                                else
                                {
                                    ErrorMessage = "Employee information not found.";
                                }
                            }
                            else
                            {
                                ErrorMessage = "User is not associated with any record.";
                            }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(Email) ||
                propertyName == nameof(Heslo) ||
                propertyName == nameof(ConfirmPassword) ||
                propertyName == nameof(IsRegistering))
            {
                
            }
        }
    }
}