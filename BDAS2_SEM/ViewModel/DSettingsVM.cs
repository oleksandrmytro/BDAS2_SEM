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
        private readonly IPriponaRepository _priponaRepository;
        private readonly IWindowService _windowService;
        private readonly Action<byte[]> _updateAvatarAction;

        private bool _isEditingFirstName;
        private bool _isEditingLastName;
        private bool _isEditingPhone;
        private bool _isEditingEmail;

        public DSettingsVM(
            IZamestnanecRepository zamestnanecRepository,
            IUzivatelDataRepository uzivatelDataRepository,
            IBlobTableRepository blobRepository,
            IPriponaRepository priponaRepository,
            IWindowService windowService,
            Action<byte[]> updateAvatarAction)
        {
            _zamestnanecRepository = zamestnanecRepository;
            _uzivatelDataRepository = uzivatelDataRepository;
            _blobRepository = blobRepository;
            _priponaRepository = priponaRepository;
            _windowService = windowService;
            _updateAvatarAction = updateAvatarAction;

            EditFirstNameCommand = new RelayCommand(ToggleEditFirstName);
            ToggleEditLastNameCommand = new RelayCommand(ToggleEditLastName);
            EditPhoneCommand = new RelayCommand(ToggleEditPhone);
            EditEmailCommand = new RelayCommand(ToggleEditEmail);
            AddOrChangeImageCommand = new RelayCommand(async _ => await AddOrChangeImage());
            SaveCommand = new RelayCommand(async _ => await SaveChanges());
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

        public string FirstName
        {
            get => _doctor?.Jmeno;
            set
            {
                if (_doctor != null && _doctor.Jmeno != value)
                {
                    _doctor.Jmeno = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName
        {
            get => _doctor?.Prijmeni;
            set
            {
                if (_doctor != null && _doctor.Prijmeni != value)
                {
                    _doctor.Prijmeni = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PhoneNumber
        {
            get => _doctor?.Telefon.ToString();
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
                if (_userData != null && _userData.Email != value)
                {
                    _userData.Email = value;
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

        // Commands
        public ICommand EditFirstNameCommand { get; }
        public ICommand ToggleEditLastNameCommand { get; }
        public ICommand EditPhoneCommand { get; }
        public ICommand EditEmailCommand { get; }
        public ICommand AddOrChangeImageCommand { get; }
        public ICommand SaveCommand { get; }

        // Methods
        private async void LoadDoctorData()
        {
            if (_doctor != null)
            {
                var doctorData = await _zamestnanecRepository.GetZamestnanecById(_doctor.IdZamestnanec);
                if (doctorData != null)
                {
                    Doctor = doctorData;

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

                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Email));
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

                    if (Doctor.BlobId != 0)
                    {
                        // Update existing blob
                        blob.IdBlob = Doctor.BlobId;
                        await _blobRepository.UpdateBlob(blob);
                    }
                    else
                    {
                        // Add new blob
                        int newBlobId = await _blobRepository.AddBlob(blob);
                        Doctor.BlobId = newBlobId;

                        // Call the stored procedure to update zamestnanec's BlobId
                        await _zamestnanecRepository.UpdateEmployeeBlob(Doctor.IdZamestnanec, newBlobId);
                        await _zamestnanecRepository.UpdateZamestnanec(Doctor); // Optional: ensure ViewModel is updated
                    }

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
                    await _zamestnanecRepository.UpdateZamestnanec(Doctor);
                }

                if (_userData != null)
                {
                    await _uzivatelDataRepository.UpdateUserData(_userData);
                }

                IsEditingFirstName = false;
                IsEditingLastName = false;
                IsEditingPhone = false;
                IsEditingEmail = false;

                // Optionally, notify the user of success

            }
            catch (Exception ex)
            {
                // Handle exceptions

            }
        }

        private void ToggleEditFirstName(object parameter)
        {
            IsEditingFirstName = !IsEditingFirstName;
            IsEditingLastName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
        }

        private void ToggleEditLastName(object parameter)
        {
            IsEditingLastName = !IsEditingLastName;
            IsEditingFirstName = false;
            IsEditingPhone = false;
            IsEditingEmail = false;
        }

        private void ToggleEditPhone(object parameter)
        {
            IsEditingPhone = !IsEditingPhone;
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingEmail = false;
        }

        private void ToggleEditEmail(object parameter)
        {
            IsEditingEmail = !IsEditingEmail;
            IsEditingFirstName = false;
            IsEditingLastName = false;
            IsEditingPhone = false;
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}