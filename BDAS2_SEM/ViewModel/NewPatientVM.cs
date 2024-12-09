using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BDAS2_SEM.View;
using BDAS2_SEM.View.PatientViews;

public class NewPatientVM : INotifyPropertyChanged
{
    private readonly IPacientRepository _pacientRepository;
    private readonly IAdresaRepository _adresaRepository;
    private readonly UZIVATEL_DATA _userData;
    private readonly IWindowService _windowService;
    private readonly Action<bool> _onClosed;

    public bool CanSave
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Jmeno) &&
                   !string.IsNullOrWhiteSpace(Prijmeni) &&
                   RodneCislo > 0 &&
                   Telefon > 0 &&
                   DatumNarozeni != DateTime.MinValue &&
                   !string.IsNullOrWhiteSpace(Pohlavi) &&
                   AdresaId > 0;
        }
    }

    public ObservableCollection<ADRESA> Addresses { get; set; }

    private string _jmeno;
    public string Jmeno
    {
        get => _jmeno;
        set
        {
            _jmeno = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private string _prijmeni;
    public string Prijmeni
    {
        get => _prijmeni;
        set
        {
            _prijmeni = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private int _rodneCislo;
    public int RodneCislo
    {
        get => _rodneCislo;
        set
        {
            _rodneCislo = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private long _telefon;
    public long Telefon
    {
        get => _telefon;
        set
        {
            _telefon = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private DateTime _datumNarozeni = DateTime.MinValue;
    public DateTime DatumNarozeni
    {
        get => _datumNarozeni;
        set
        {
            _datumNarozeni = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private string _pohlavi;
    public string Pohlavi
    {
        get => _pohlavi;
        set
        {
            _pohlavi = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private int _adresaId;
    public int AdresaId
    {
        get => _adresaId;
        set
        {
            _adresaId = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
        }
    }

    // Команди
    public ICommand SaveCommand { get; }
    public ICommand AddAddressCommand { get; }

    public NewPatientVM(UZIVATEL_DATA userData, IWindowService windowService, IServiceProvider serviceProvider, Action<bool> onClosed)
    {
        _userData = userData;
        _windowService = windowService;
        _onClosed = onClosed;

        _pacientRepository = serviceProvider.GetRequiredService<IPacientRepository>();
        _adresaRepository = serviceProvider.GetRequiredService<IAdresaRepository>();

        SaveCommand = new RelayCommand(Save);
        AddAddressCommand = new RelayCommand(AddAddress);

        LoadAddresses();
    }

    private async void LoadAddresses()
    {
        var addresses = await _adresaRepository.GetAllAddresses();
        Addresses = new ObservableCollection<ADRESA>(addresses);
        OnPropertyChanged(nameof(Addresses));
    }

    private async void Save(object parameter)
    {
        var pacient = new PACIENT
        {
            Jmeno = this.Jmeno,
            Prijmeni = this.Prijmeni,
            RodneCislo = this.RodneCislo,
            Telefon = this.Telefon,
            DatumNarozeni = this.DatumNarozeni,
            Pohlavi = this.Pohlavi,
            AdresaId = this.AdresaId,
            UserDataId = _userData.Id
        };

        await _pacientRepository.AddPacient(pacient);

        _onClosed?.Invoke(true);

        _windowService.CloseWindow(() =>
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is NewPatientWindow)
                {
                    window.Close();
                    break;
                }
            }
        });
    }

    private void AddAddress(object parameter)
    {
        _windowService.OpenAddAddressWindow(OnAddressAdded);
    }

    private void OnAddressAdded(ADRESA newAddress)
    {
        Addresses.Add(newAddress);
        AdresaId = newAddress.IdAdresa;
        OnPropertyChanged(nameof(Addresses));
        OnPropertyChanged(nameof(AdresaId));
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