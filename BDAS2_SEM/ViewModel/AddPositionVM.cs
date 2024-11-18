using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

public class AddPositionVM : INotifyPropertyChanged
{
    private readonly IPoziceRepository _poziceRepository;
    private readonly IWindowService _windowService;
    private readonly Action<POZICE> _onPositionAdded;

    public string Nazev { get; set; }

    public ICommand SaveCommand { get; }

    public AddPositionVM(Action<POZICE> onPositionAdded, IWindowService windowService, IPoziceRepository poziceRepository)
    {
        _onPositionAdded = onPositionAdded;
        _windowService = windowService;
        _poziceRepository = poziceRepository;
        SaveCommand = new RelayCommand(Save);
    }

    private async void Save(object parameter)
    {
        var newPozice = new POZICE
        {
            Nazev = this.Nazev
        };

        int id = await _poziceRepository.AddPozice(newPozice);
        newPozice.IdPozice = id;

        _onPositionAdded?.Invoke(newPozice);

        // Close the window
        _windowService.CloseWindow(() =>
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is BDAS2_SEM.View.AdminViews.AddPositionWindow)
                {
                    window.Close();
                    break;
                }
            }
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}