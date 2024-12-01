using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class PSettingsVM : INotifyPropertyChanged
{
    private readonly IWindowService _windowService;
    private readonly IPatientContextService _patientContextService;

    public ICommand EditDataCommand { get; }

    public PSettingsVM(IWindowService windowService, IPatientContextService patientContextService)
    {
        _windowService = windowService;
        _patientContextService = patientContextService;
        EditDataCommand = new RelayCommand(EditData);
    }

    private void EditData(object parameter)
    {
        var currentPatient = _patientContextService.CurrentPatient;
        if (currentPatient != null)
        {
            _windowService.OpenEditPatientWindow(currentPatient);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}