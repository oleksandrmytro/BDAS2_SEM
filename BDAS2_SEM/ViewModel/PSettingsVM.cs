using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BDAS2_SEM.View;

namespace BDAS2_SEM.ViewModel
{
    public class PSettingsVM : INotifyPropertyChanged
    {
        private readonly IWindowService _windowService;
        private readonly IPacientRepository _pacientRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IPatientContextService _patientContextService;
        private readonly IAdresaRepository _adresaRepository; // Репозиторій для адрес

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

        // Нові властивості для адреси
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
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public PSettingsVM(
            IWindowService windowService,
            IPatientContextService patientContextService,
            IPacientRepository pacientRepository,
            IUzivatelDataRepository uzivatelDataRepository,
            IAdresaRepository adresaRepository) // Додайте репозиторій адрес
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
            SaveCommand = new RelayCommand(async _ => await SaveChanges());
            CancelCommand = new RelayCommand(_ => CancelEdit());

            AddAddressCommand = new RelayCommand(async _ => await AddNewAddress());
            EditAddressCommand = new RelayCommand(_ => ToggleEditingAddress());

            AddressList = new ObservableCollection<ADRESA>();

            LoadData();
        }

        private void ToggleEditingAddress()
        {
            IsEditingAddress = !IsEditingAddress;

            if (IsEditingAddress)
            {
                // Disable other edit modes if necessary
                IsEditingFirstName = false;
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;
            }
            else
            {
                // Reset the selected address if editing is canceled
                SelectedAddress = AddressList.FirstOrDefault(a => a.IdAdresa == _pacient.AdresaId);
            }
        }

        private async void LoadData()
        {
            int userId = _patientContextService.CurrentPatient.UserDataId;

            _userData = await _uzivatelDataRepository.GetUzivatelById(userId);
            _pacient = await _pacientRepository.GetPacientByUserDataId(userId);

            // Load the list of addresses first
            await LoadAddressList();

            if (_pacient != null)
            {
                FirstName = _pacient.Jmeno;
                LastName = _pacient.Prijmeni;
                PhoneNumber = _pacient.Telefon.ToString();

                // Set the SelectedAddress to the user's address
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
                // Вимикаємо інші поля редагування, якщо потрібно
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;
            }
            else
            {
                // Скидаємо значення, якщо редагування відмінено
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
            }
            else
            {
                Email = _userData.Email;
            }
        }

        private void CancelEdit()
        {
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
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

                if (IsEditingAddress)
                {
                    _pacient.AdresaId = SelectedAddress?.IdAdresa ?? 0;
                    IsEditingAddress = false;
                }

                await _pacientRepository.UpdatePacient(_pacient);

                if (IsEditingPhone)
                {
                    if (long.TryParse(PhoneNumber, out long phone))
                    {
                        _pacient.Telefon = phone;
                    }
                    else
                    {
                        // Обробка некоректного введення номера телефону
                        _pacient.Telefon = 1;
                    }
                    IsEditingPhone = false;
                }

                // Оновлюємо адресу, якщо змінилася
                if (_pacient.AdresaId != (SelectedAddress?.IdAdresa ?? 0))
                {
                    _pacient.AdresaId = SelectedAddress?.IdAdresa ?? 0;
                }

                await _pacientRepository.UpdatePacient(_pacient);
            }

            if (_userData != null && IsEditingEmail)
            {
                _userData.Email = Email;
                IsEditingEmail = false;

                await _uzivatelDataRepository.UpdateUserData(_userData);
            }

            // Перезавантажуємо дані, щоб відобразити зміни
            LoadData();
        }

        private async Task AddNewAddress()
        {
            // Відкриваємо вікно для додавання нової адреси з передачею колбеку
            _windowService.OpenAddAddressWindow(async newAddress =>
            {
                if (newAddress != null)
                {
                    // Додаємо нову адресу до бази даних
                    await _adresaRepository.AddAdresa(newAddress);

                    // Додаємо нову адресу до списку
                    AddressList.Add(newAddress);

                    // Встановлюємо нову адресу як вибрану
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
