using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View.PatientViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class EditPatientVM : INotifyPropertyChanged
    {
        private PACIENT _patient;
        private readonly IPacientRepository _pacientRepository;
        private readonly IAdresaRepository _adresaRepository;
        private readonly IWindowService _windowService;
        private readonly Action<bool> _onClosed;

        public EditPatientVM(PACIENT patient, IPacientRepository pacientRepository, IAdresaRepository adresaRepository, IWindowService windowService, Action<bool> onClosed)
        {
            _patient = patient;
            _pacientRepository = pacientRepository;
            _adresaRepository = adresaRepository;
            _windowService = windowService;
            _onClosed = onClosed;
            LoadPatientData();
            LoadAddresses();
            SaveCommand = new RelayCommand(SavePatient);
            AddAddressCommand = new RelayCommand(AddAddress);
        }

        public PACIENT Patient
        {
            get => _patient;
            set
            {
                _patient = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ADRESA> Addresses { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand AddAddressCommand { get; }

        private async void LoadPatientData()
        {
            var patientData = await _pacientRepository.GetPacientById(_patient.IdPacient);
            if (patientData != null)
            {
                Patient = patientData;
            }
        }

        private async void LoadAddresses()
        {
            var addresses = await _adresaRepository.GetAllAddresses();
            Addresses = new ObservableCollection<ADRESA>(addresses);
            OnPropertyChanged(nameof(Addresses));
        }

        private async void SavePatient(object parameter)
        {
            Debug.WriteLine("SavePatient called");
            await CompareAndSavePatient();
            _onClosed?.Invoke(true);
            _windowService.CloseWindow(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is EditPatientWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            });
        }

        private async Task CompareAndSavePatient()
        {
            Debug.WriteLine("CompareAndSavePatient called");
            var patientData = await _pacientRepository.GetPacientById(_patient.IdPacient);
            if (patientData != null && !patientData.Equals(Patient))
            {
                Debug.WriteLine("Data has changed, calling UpdatePacient");
                await _pacientRepository.UpdatePacient(Patient);
            }
            else
            {
                Debug.WriteLine("No changes detected");
            }
        }

        private void AddAddress(object parameter)
        {
            _windowService.OpenAddAddressWindow(OnAddressAdded);
        }

        private void OnAddressAdded(ADRESA newAddress)
        {
            Addresses.Add(newAddress);
            Patient.AdresaId = newAddress.IdAdresa;
            OnPropertyChanged(nameof(Addresses));
            OnPropertyChanged(nameof(Patient.AdresaId));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void InvokeOnClosed(bool isSaved)
        {
            _onClosed?.Invoke(isSaved);
        }
    }
}