using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;
using System.IO;
using System.Threading.Tasks;
using BDAS2_SEM.Services.Interfaces;

namespace BDAS2_SEM.ViewModel
{
    public class DSettingsVM : INotifyPropertyChanged
    {
        private ZAMESTNANEC _doctor;
        private UZIVATEL_DATA _userData;
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IBlobTableRepository _blobRepository;
        private readonly IWindowService _windowService;
        private readonly Action<byte[]> _updateAvatarAction;

        private bool _isEditingName;
        private bool _isEditingPhone;
        private bool _isEditingEmail;

        public DSettingsVM(
            IZamestnanecRepository zamestnanecRepository,
            IUzivatelDataRepository uzivatelDataRepository,
            IBlobTableRepository blobRepository,
            IWindowService windowService,
            Action<byte[]> updateAvatarAction)
        {
            _zamestnanecRepository = zamestnanecRepository;
            _uzivatelDataRepository = uzivatelDataRepository;
            _blobRepository = blobRepository;
            _windowService = windowService;
            _updateAvatarAction = updateAvatarAction;

            EditNameCommand = new RelayCommand(ToggleEditName);
            EditPhoneCommand = new RelayCommand(ToggleEditPhone);
            EditEmailCommand = new RelayCommand(ToggleEditEmail);
            AddOrChangeImageCommand = new RelayCommand(AddOrChangeImage);
            SaveCommand = new RelayCommand(async _ => await SaveChanges());

        }

        public void SetDoctor(ZAMESTNANEC doctor)
        {
            _doctor = doctor;
            LoadDoctorData();
        }

        // Властивості
        public ZAMESTNANEC Doctor
        {
            get => _doctor;
            set
            {
                _doctor = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => $"{_doctor.Jmeno} {_doctor.Prijmeni}";
            set
            {
                var parts = value.Split(' ');
                if (parts.Length > 0) _doctor.Jmeno = parts[0];
                if (parts.Length > 1) _doctor.Prijmeni = parts[1];
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _doctor.Telefon.ToString();
            set
            {
                if (long.TryParse(value, out var phone))
                {
                    _doctor.Telefon = phone;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => _userData?.Email;
            set
            {
                if (_userData != null)
                {
                    _userData.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEditingName
        {
            get => _isEditingName;
            set
            {
                _isEditingName = value;
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

        public bool IsEditingEmail { get => _isEditingEmail; set { _isEditingEmail = value; OnPropertyChanged(); } }

        // Команди
        public ICommand EditNameCommand { get; }
        public ICommand EditPhoneCommand { get; }
        public ICommand EditEmailCommand { get; }
        public ICommand AddOrChangeImageCommand { get; }
        public ICommand SaveCommand { get; }


        // Методи
        private async void LoadDoctorData()
        {
            if (_doctor != null)
            {
                var doctorData = await _zamestnanecRepository.GetZamestnanecById(_doctor.IdZamestnanec);
                var userData = await _uzivatelDataRepository.GetUzivatelById(_doctor.UserDataId);

                if (doctorData != null) _doctor = doctorData;
                if (userData != null) _userData = userData;

                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Email));
            }
        }

        private async void AddOrChangeImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                byte[] imageData = File.ReadAllBytes(openFileDialog.FileName);
                _updateAvatarAction?.Invoke(imageData);
            }
        }

        private void ToggleEditName(object parameter)
        {
            IsEditingName = !IsEditingName;
            IsEditingPhone = false;
            IsEditingEmail = false;
        }

        private void ToggleEditPhone(object parameter)
        {
            IsEditingPhone = !IsEditingPhone;
            IsEditingName = false;
            IsEditingEmail = false;
        }

        private void ToggleEditEmail(object parameter)
        {
            IsEditingEmail = !IsEditingEmail;
            IsEditingName = false;
            IsEditingPhone = false;
        }

        private async Task SaveChanges()
        {
            if (_doctor != null)
            {
                await _zamestnanecRepository.UpdateZamestnanec(_doctor);
            }
            if (_userData != null)
            {
                await _uzivatelDataRepository.UpdateUserData(_userData);
            }

            IsEditingName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
        }

        // Реалізація INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
