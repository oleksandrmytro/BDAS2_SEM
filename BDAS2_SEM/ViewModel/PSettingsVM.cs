using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using BDAS2_SEM.View;
using System.Text;
using Xceed.Wpf.Toolkit;
using System.Linq;

namespace BDAS2_SEM.ViewModel
{
    public class PSettingsVM : INotifyPropertyChanged
    {
        private readonly IWindowService _windowService;
        private readonly IPacientRepository _pacientRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IPatientContextService _patientContextService;
        private readonly IAdresaRepository _adresaRepository;

        private PACIENT _pacient;
        private UZIVATEL_DATA _userData;

        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private string _email;

        private bool _isEditingFirstName;
        private bool _isEditingLastName;
        private bool _isEditingPhone;
        private bool _isEditingEmail;
        private bool _isEditingPassword;

        // Властивості пароля
        private string _currentPassword;
        public string CurrentPassword
        {
            get => _currentPassword;
            set { _currentPassword = value; OnPropertyChanged(); }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        private string _confirmNewPassword;
        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set { _confirmNewPassword = value; OnPropertyChanged(); }
        }

        // Властивості адреси
        private ObservableCollection<ADRESA> _addressList;
        public ObservableCollection<ADRESA> AddressList
        {
            get => _addressList;
            set
            {
                _addressList = value;
                OnPropertyChanged();
            }
        }

        private ADRESA _selectedAddress;
        public ADRESA SelectedAddress
        {
            get => _selectedAddress;
            set
            {
                if (_selectedAddress != value)
                {
                    _selectedAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddAddressCommand { get; }
        public ICommand EditAddressCommand { get; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEditingFirstName
        {
            get => _isEditingFirstName;
            set
            {
                _isEditingFirstName = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditingLastName
        {
            get => _isEditingLastName;
            set
            {
                _isEditingLastName = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditingPhone
        {
            get => _isEditingPhone;
            set
            {
                _isEditingPhone = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditingEmail
        {
            get => _isEditingEmail;
            set
            {
                _isEditingEmail = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditingPassword
        {
            get => _isEditingPassword;
            set
            {
                _isEditingPassword = value;
                OnPropertyChanged();
            }
        }

        private bool _isEditingAddress;
        public bool IsEditingAddress
        {
            get => _isEditingAddress;
            set
            {
                _isEditingAddress = value;
                OnPropertyChanged();
            }
        }

        public ICommand EditFirstNameCommand { get; }
        public ICommand EditLastNameCommand { get; }
        public ICommand EditPhoneCommand { get; }
        public ICommand EditEmailCommand { get; }
        public ICommand EditPasswordCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        public PSettingsVM(
            IWindowService windowService,
            IPatientContextService patientContextService,
            IPacientRepository pacientRepository,
            IUzivatelDataRepository uzivatelDataRepository,
            IAdresaRepository adresaRepository)
        {
            _windowService = windowService;
            _pacientRepository = pacientRepository;
            _uzivatelDataRepository = uzivatelDataRepository;
            _patientContextService = patientContextService;
            _adresaRepository = adresaRepository;

            EditFirstNameCommand = new RelayCommand(_ => ToggleEditingFirstName());
            EditLastNameCommand = new RelayCommand(_ => ToggleEditingLastName());
            EditPhoneCommand = new RelayCommand(_ => ToggleEditingPhone());
            EditEmailCommand = new RelayCommand(_ => ToggleEditingEmail());
            EditPasswordCommand = new RelayCommand(_ => ToggleEditingPassword());
            SaveCommand = new RelayCommand(async _ => await SaveChanges());
            CancelCommand = new RelayCommand(_ => CancelEdit());
            ChangePasswordCommand = new RelayCommand(async _ => await ChangePassword());

            AddAddressCommand = new RelayCommand(async _ => await AddNewAddress());
            EditAddressCommand = new RelayCommand(_ => ToggleEditingAddress());

            AddressList = new ObservableCollection<ADRESA>();

            LoadData();
        }

        private async void LoadData()
        {
            int userId = _patientContextService.CurrentPatient.UserDataId;

            _userData = await _uzivatelDataRepository.GetUzivatelById(userId);
            _pacient = await _pacientRepository.GetPacientByUserDataId(userId);

            await LoadAddressList();

            if (_pacient != null)
            {
                FirstName = _pacient.Jmeno;
                LastName = _pacient.Prijmeni;
                PhoneNumber = _pacient.Telefon.ToString();

                if (_pacient.AdresaId != 0)
                {
                    SelectedAddress = AddressList.FirstOrDefault(a => a.IdAdresa == _pacient.AdresaId);
                }
            }

            if (_userData != null)
            {
                Email = _userData.Email;
            }

            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(SelectedAddress));
        }

        private async Task LoadAddressList()
        {
            var addresses = await _adresaRepository.GetAllAddresses();
            AddressList.Clear();
            foreach (var address in addresses)
            {
                AddressList.Add(address);
            }
        }

        private void ToggleEditingFirstName()
        {
            IsEditingFirstName = !IsEditingFirstName;

            if (IsEditingFirstName)
            {
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;
                IsEditingPassword = false;
                IsEditingAddress = false;
            }
            else
            {
                FirstName = _pacient.Jmeno;
            }
        }

        private void ToggleEditingLastName()
        {
            IsEditingLastName = !IsEditingLastName;

            if (IsEditingLastName)
            {
                IsEditingFirstName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;
                IsEditingPassword = false;
                IsEditingAddress = false;
            }
            else
            {
                LastName = _pacient.Prijmeni;
            }
        }

        private void ToggleEditingPhone()
        {
            IsEditingPhone = !IsEditingPhone;

            if (IsEditingPhone)
            {
                IsEditingFirstName = false;
                IsEditingLastName = false;
                IsEditingEmail = false;
                IsEditingPassword = false;
                IsEditingAddress = false;
            }
            else
            {
                PhoneNumber = _pacient.Telefon.ToString();
            }
        }

        private void ToggleEditingEmail()
        {
            IsEditingEmail = !IsEditingEmail;

            if (IsEditingEmail)
            {
                IsEditingFirstName = false;
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingPassword = false;
                IsEditingAddress = false;
            }
            else
            {
                Email = _userData.Email;
            }
        }

        private void ToggleEditingPassword()
        {
            IsEditingPassword = !IsEditingPassword;

            if (IsEditingPassword)
            {
                IsEditingFirstName = false;
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;
                IsEditingAddress = false;
            }
            else
            {
                // Скидаємо поля пароля при відміні редагування
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
            }
        }

        private void ToggleEditingAddress()
        {
            IsEditingAddress = !IsEditingAddress;

            if (IsEditingAddress)
            {
                IsEditingFirstName = false;
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;
                IsEditingPassword = false;
            }
            else
            {
                SelectedAddress = AddressList.FirstOrDefault(a => a.IdAdresa == _pacient.AdresaId);
            }
        }

        private void CancelEdit()
        {
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
            IsEditingPassword = false;
            IsEditingAddress = false;
            LoadData();
        }

        private async Task SaveChanges()
        {
            if (_pacient != null)
            {
                if (IsEditingFirstName)
                {
                    _pacient.Jmeno = FirstName;
                    IsEditingFirstName = false;
                }

                if (IsEditingLastName)
                {
                    _pacient.Prijmeni = LastName;
                    IsEditingLastName = false;
                }

                if (IsEditingPhone)
                {
                    if (long.TryParse(PhoneNumber, out long phone))
                    {
                        _pacient.Telefon = phone;
                    }
                    else
                    {
                        MessageBox.Show("Невірний формат номера телефону.");
                        return;
                    }
                    IsEditingPhone = false;
                }

                if (IsEditingAddress)
                {
                    _pacient.AdresaId = SelectedAddress?.IdAdresa ?? 0;
                    IsEditingAddress = false;
                }

                await _pacientRepository.UpdatePacient(_pacient);
            }

            if (_userData != null)
            {
                if (IsEditingEmail)
                {
                    _userData.Email = Email;
                    IsEditingEmail = false;
                    await _uzivatelDataRepository.UpdateUserData(_userData);
                }

                if (IsEditingPassword)
                {
                    await ChangePassword();
                    IsEditingPassword = false;
                }
            }

            LoadData();
        }

        private async Task ChangePassword()
        {
            try
            {
                string oldPassword1 = HashPassword("123");
                if (string.IsNullOrEmpty(CurrentPassword))
                {
                    MessageBox.Show("Будь ласка, введіть поточний пароль.");
                    return;
                }

                if (_userData.Heslo != HashPassword(CurrentPassword))
                {
                    MessageBox.Show("Поточний пароль невірний.");
                    return;
                }

                if (string.IsNullOrEmpty(NewPassword))
                {
                    MessageBox.Show("Будь ласка, введіть новий пароль.");
                    return;
                }

                if (NewPassword != ConfirmNewPassword)
                {
                    MessageBox.Show("Новий пароль та підтвердження не співпадають.");
                    return;
                }

                string newPasswordHash = HashPassword(NewPassword);
                string oldPassword = HashPassword(_userData.Heslo);
                _userData.Heslo = newPasswordHash;
                await _uzivatelDataRepository.UpdateUserData(_userData);

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;

                MessageBox.Show("Пароль успішно змінено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private async Task AddNewAddress()
        {
            _windowService.OpenAddAddressWindow(async newAddress =>
            {
                if (newAddress != null)
                {
                    await _adresaRepository.AddAdresa(newAddress);
                    AddressList.Add(newAddress);
                    SelectedAddress = newAddress;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}