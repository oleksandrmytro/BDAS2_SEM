using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;
using System.IO;
using System.Threading.Tasks;
using BDAS2_SEM.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace BDAS2_SEM.ViewModel
{
    public class DSettingsVM : INotifyPropertyChanged
    {
        private ZAMESTNANEC _doctor;
        private UZIVATEL_DATA _userData;
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IBlobTableRepository _blobRepository;
        private readonly IPriponaRepository _priponaRepository;
        private readonly IAdresaRepository _adresaRepository;
        private readonly IWindowService _windowService;
        private readonly Action<byte[]> _updateAvatarAction;

        private bool _isEditingFirstName;
        private bool _isEditingLastName;
        private bool _isEditingPhone;
        private bool _isEditingEmail;
        private bool _isEditingAddress;
        private bool _isEditingPassword;

        // Properties for address editing
        private ObservableCollection<ADRESA> _addressList;
        public ObservableCollection<ADRESA> AddressList
        {
            get => _addressList;
            set { _addressList = value; OnPropertyChanged(); }
        }

        private ADRESA _selectedAddress;
        public ADRESA SelectedAddress
        {
            get => _selectedAddress;
            set { _selectedAddress = value; OnPropertyChanged(); }
        }

        public bool IsEditingAddress
        {
            get => _isEditingAddress;
            set { _isEditingAddress = value; OnPropertyChanged(); }
        }

        // Properties for password change
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

        public bool IsEditingPassword
        {
            get => _isEditingPassword;
            set { _isEditingPassword = value; OnPropertyChanged(); }
        }

        // Existing properties
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public bool IsEditingFirstName
        {
            get => _isEditingFirstName;
            set { _isEditingFirstName = value; OnPropertyChanged(); }
        }

        public bool IsEditingLastName
        {
            get => _isEditingLastName;
            set { _isEditingLastName = value; OnPropertyChanged(); }
        }

        public bool IsEditingPhone
        {
            get => _isEditingPhone;
            set { _isEditingPhone = value; OnPropertyChanged(); }
        }

        public bool IsEditingEmail
        {
            get => _isEditingEmail;
            set { _isEditingEmail = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand EditFirstNameCommand { get; }
        public ICommand ToggleEditLastNameCommand { get; }
        public ICommand EditPhoneCommand { get; }
        public ICommand EditEmailCommand { get; }
        public ICommand AddOrChangeImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ToggleEditingPasswordCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand CancelPasswordChangeCommand { get; }

        // Commands for address editing
        public ICommand EditAddressCommand { get; }
        public ICommand AddAddressCommand { get; }

        public DSettingsVM(
            IZamestnanecRepository zamestnanecRepository,
            IUzivatelDataRepository uzivatelDataRepository,
            IBlobTableRepository blobRepository,
            IPriponaRepository priponaRepository,
            IAdresaRepository adresaRepository,
            IWindowService windowService,
            Action<byte[]> updateAvatarAction)
        {
            _zamestnanecRepository = zamestnanecRepository;
            _uzivatelDataRepository = uzivatelDataRepository;
            _blobRepository = blobRepository;
            _priponaRepository = priponaRepository;
            _adresaRepository = adresaRepository;
            _windowService = windowService;
            _updateAvatarAction = updateAvatarAction;

            EditFirstNameCommand = new RelayCommand(ToggleEditFirstName);
            ToggleEditLastNameCommand = new RelayCommand(ToggleEditLastName);
            EditPhoneCommand = new RelayCommand(ToggleEditPhone);
            EditEmailCommand = new RelayCommand(ToggleEditEmail);
            AddOrChangeImageCommand = new RelayCommand(async _ => await AddOrChangeImage());
            SaveCommand = new RelayCommand(async _ => await SaveChanges());
            ToggleEditingPasswordCommand = new RelayCommand(ToggleEditingPassword);
            ChangePasswordCommand = new RelayCommand(async _ => await ChangePassword());
            CancelPasswordChangeCommand = new RelayCommand(CancelPasswordChange);

            EditAddressCommand = new RelayCommand(ToggleEditingAddress);
            AddAddressCommand = new RelayCommand(async _ => await AddNewAddress());

            AddressList = new ObservableCollection<ADRESA>();

            LoadDoctorData();
        }

        public void SetDoctor(ZAMESTNANEC doctor)
        {
            _doctor = doctor;
            LoadDoctorData();
        }

        // Properties
        public ZAMESTNANEC Doctor
        {
            get => _doctor;
            set
            {
                _doctor = value;
                OnPropertyChanged();
            }
        }

        // Methods
        private async void LoadDoctorData()
        {
            if (_doctor != null)
            {
                await LoadAddressList();
                var doctorData = await _zamestnanecRepository.GetZamestnanecById(_doctor.IdZamestnanec);
                if (doctorData != null)
                {
                    Doctor = doctorData;

                    FirstName = Doctor.Jmeno;
                    LastName = Doctor.Prijmeni;
                    PhoneNumber = Doctor.Telefon.ToString();

                    if (Doctor.AdresaId != 0)
                    {
                        SelectedAddress = AddressList.FirstOrDefault(a => a.IdAdresa == Doctor.AdresaId);
                    }

                    // Load image if BlobId is set
                    if (Doctor.BlobId != 0)
                    {
                        var blob = await _blobRepository.GetBlobById(Doctor.BlobId);
                        if (blob != null && blob.Obsah != null)
                        {
                            _updateAvatarAction?.Invoke(blob.Obsah);
                        }
                    }
                }

                _userData = await _uzivatelDataRepository.GetUzivatelById(_doctor.UserDataId);

                Email = _userData.Email;

                // Notify property changes
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(SelectedAddress));
            }
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

        private async Task AddOrChangeImage()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.png;*.jpg;*.jpeg"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    byte[] imageData = File.ReadAllBytes(openFileDialog.FileName);
                    _updateAvatarAction?.Invoke(imageData);

                    // Get file extension
                    string extension = Path.GetExtension(openFileDialog.FileName).ToLower();

                    // Determine the PRIPONA type based on the file extension
                    string priponaTyp = extension switch
                    {
                        ".png" => "png",
                        ".jpg" => "jpg",
                        ".jpeg" => "jpeg",
                        _ => "UNKNOWN"
                    };

                    // Get or create PRIPONA
                    var pripona = await _priponaRepository.GetPriponaByTyp(priponaTyp);
                    if (pripona == null)
                    {
                        pripona = new PRIPONA { Typ = priponaTyp };
                        pripona.IdPripona = await _priponaRepository.AddPripona(pripona);
                    }

                    BLOB_TABLE blob = new BLOB_TABLE
                    {
                        NazevSouboru = Path.GetFileName(openFileDialog.FileName),
                        TypSouboru = "Image",
                        Obsah = imageData,
                        DatumNahrani = DateTime.Now,
                        DatumModifikace = DateTime.Now,
                        OperaceProvedl = $"{_doctor.Jmeno} {_doctor.Prijmeni}",
                        PopisOperace = "Adding avatar",
                        PriponaId = pripona.IdPripona // Set the PRIPONA ID
                    };

                    int oldBlob = Doctor.BlobId;

                    if (oldBlob != 1)
                    {
                        await _zamestnanecRepository.UpdateEmployeeBlob(Doctor.IdZamestnanec, 1);
                        await _blobRepository.DeleteBlob(oldBlob); // Delete old blob
                    }
                    
                        // Add new blob
                        int newBlobId = await _blobRepository.AddBlob(blob);
                        Doctor.BlobId = newBlobId;

                        // Call the stored procedure to update zamestnanec's BlobId
                        await _zamestnanecRepository.UpdateEmployeeBlob(Doctor.IdZamestnanec, newBlobId);
                        await _zamestnanecRepository.UpdateZamestnanec(Doctor); // Optional: ensure ViewModel is updated
                        LoadDoctorData();
                    

                    // Optionally, notify the user of success
                }
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex) when (ex.Number == 2291)
            {
                // Handle foreign key constraint violation (ORA-02291)
            }
            catch (Exception ex)
            {
                // Handle other exceptions
            }
  
        }

        private async Task SaveChanges()
        {
            try
            {
                if (Doctor != null)
                {
                    // Update Doctor properties with values from the ViewModel
                    Doctor.Jmeno = FirstName;
                    Doctor.Prijmeni = LastName;

                    if (long.TryParse(PhoneNumber, out var phone))
                    {
                        Doctor.Telefon = phone;
                    }
                    else
                    {
                        Doctor.Telefon = 0; // Or handle invalid input accordingly
                    }

                    // Update the Doctor's AdresaId with the SelectedAddress
                    Doctor.AdresaId = SelectedAddress?.IdAdresa ?? 0;

                    await _zamestnanecRepository.UpdateZamestnanec(Doctor);
                }

                if (_userData != null)
                {
                    // Update user data properties
                    _userData.Email = Email;
                    await _uzivatelDataRepository.UpdateUserData(_userData);
                }

                // Reset editing states
                IsEditingFirstName = false;
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;
                IsEditingAddress = false;
                IsEditingPassword = false;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
            }
        }

        private void ToggleEditFirstName(object parameter)
        {
            IsEditingFirstName = !IsEditingFirstName;
            IsEditingLastName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
            IsEditingPassword = false;
        }

        private void ToggleEditLastName(object parameter)
        {
            IsEditingLastName = !IsEditingLastName;
            IsEditingFirstName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
            IsEditingPassword = false;
        }

        private void ToggleEditPhone(object parameter)
        {
            IsEditingPhone = !IsEditingPhone;
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingEmail = false;
            IsEditingPassword = false;
        }

        private void ToggleEditEmail(object parameter)
        {
            IsEditingEmail = !IsEditingEmail;
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingPhone = false;
            IsEditingPassword = false;
        }

        private void ToggleEditingAddress(object parameter)
        {
            IsEditingAddress = !IsEditingAddress;
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
            IsEditingPassword = false;

            if (!IsEditingAddress)
            {
                // Reset to the original address if editing is canceled
                SelectedAddress = AddressList.FirstOrDefault(a => a.IdAdresa == Doctor.AdresaId);
            }
        }

        private void ToggleEditingPassword(object parameter)
        {
            IsEditingPassword = !IsEditingPassword;
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
            IsEditingAddress = false;

            if (!IsEditingPassword)
            {
                // Reset password fields if editing is canceled
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
            }
        }

        private async Task AddNewAddress()
        {
            // Assuming OpenAddAddressWindow accepts a callback for the new address
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

        private async Task ChangePassword()
        {
            try
            {
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
                _userData.Heslo = newPasswordHash;
                await _uzivatelDataRepository.UpdateUserData(_userData);

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;

                MessageBox.Show("Пароль успішно змінено.");
                IsEditingPassword = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка: {ex.Message}");
            }
        }

        private void CancelPasswordChange(object parameter)
        {
            IsEditingPassword = false;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
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

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}