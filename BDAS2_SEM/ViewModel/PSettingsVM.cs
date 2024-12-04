using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;

namespace BDAS2_SEM.ViewModel
{
    public class PSettingsVM : INotifyPropertyChanged
    {
        private readonly IWindowService _windowService;
        private readonly IPacientRepository _pacientRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IPatientContextService _patientContextService;

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
            IUzivatelDataRepository uzivatelDataRepository)
        {
            _windowService = windowService;
            _pacientRepository = pacientRepository;
            _uzivatelDataRepository = uzivatelDataRepository;
            _patientContextService = patientContextService;

            EditFirstNameCommand = new RelayCommand(_ => ToggleEditingFirstName());
            EditLastNameCommand = new RelayCommand(_ => ToggleEditingLastName());
            EditPhoneCommand = new RelayCommand(_ => ToggleEditingPhone());
            EditEmailCommand = new RelayCommand(_ => ToggleEditingEmail());
            SaveCommand = new RelayCommand(async _ => await SaveChanges());
            CancelCommand = new RelayCommand(_ => CancelEdit());

            LoadData();
        }

        private async void LoadData()
        {
            int userId = _patientContextService.CurrentPatient.UserDataId;

            _userData = await _uzivatelDataRepository.GetUzivatelById(userId);
            _pacient = await _pacientRepository.GetPacientByUserDataId(userId);

            if (_pacient != null)
            {
                FirstName = _pacient.Jmeno;
                LastName = _pacient.Prijmeni;
                PhoneNumber = _pacient.Telefon.ToString();
            }

            if (_userData != null)
            {
                Email = _userData.Email;
            }

            OnPropertyChanged(nameof(FirstName));
            OnPropertyChanged(nameof(LastName));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Email));
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
                        // Обробка некоректного введення номера телефону
                        _pacient.Telefon = 1;
                    }
                    IsEditingPhone = false;
                }

                // Переконайтеся, що інші необхідні поля заповнені

                await _pacientRepository.UpdatePacient(_pacient);
            }

            if (_userData != null && IsEditingEmail)
            {
                _userData.Email = Email;
                IsEditingEmail = false;

                // Переконайтеся, що інші необхідні поля заповнені

                await _uzivatelDataRepository.UpdateUserData(_userData);
            }

            // Перезавантажуємо дані, щоб відобразити зміни
            LoadData();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}