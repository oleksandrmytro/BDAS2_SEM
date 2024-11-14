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

public class NewPatientVM : INotifyPropertyChanged
{
    private readonly IPacientRepository _pacientRepository;
    private readonly IAdresaRepository _adresaRepository;
    private readonly UZIVATEL_DATA _userData;
    private readonly IWindowService _windowService;
    private readonly Action<bool> _onClosed;

    public ObservableCollection<ADRESA> Addresses { get; set; }

    // Властивості пацієнта
    public string Jmeno { get; set; }
    public string Prijmeni { get; set; }
    public int RodneCislo { get; set; }
    public long Telefon { get; set; }
    public DateTime DatumNarozeni { get; set; }
    public string Pohlavi { get; set; }
    public int AdresaId { get; set; }

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