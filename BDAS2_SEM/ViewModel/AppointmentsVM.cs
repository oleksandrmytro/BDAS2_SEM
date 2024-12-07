using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.ViewModel;

public class AppointmentsVM : INotifyPropertyChanged
{
    private readonly IMistnostRepository _mistnostRepository;
    private readonly INavstevaRepository _navstevaRepository;
    private readonly IWindowService _windowService;
    private ZAMESTNANEC _doctor;

    // Constructor
    public AppointmentsVM(IServiceProvider serviceProvider, IWindowService windowService)
    {
        _navstevaRepository = serviceProvider.GetRequiredService<INavstevaRepository>();
        _mistnostRepository = serviceProvider.GetRequiredService<IMistnostRepository>();
        _windowService = windowService;
        AppointmentRequests = new ObservableCollection<dynamic>();
        FutureAppointments = new ObservableCollection<dynamic>();
        PastAppointments = new ObservableCollection<dynamic>();

        AcceptAppointmentCommand = new RelayCommand(AcceptAppointment);
        CancelAppointmentCommand = new RelayCommand(CancelAppointment);
        AssignAppointmentCommand = new RelayCommand(AssignAppointment);
        UpdateAppointmentCommand = new RelayCommand(UpdateAppointment);
    }

    public ObservableCollection<dynamic> AppointmentRequests { get; set; }
    public ObservableCollection<dynamic> FutureAppointments { get; set; }
    public ObservableCollection<dynamic> PastAppointments { get; set; }

    public ICommand AcceptAppointmentCommand { get; }
    public ICommand CancelAppointmentCommand { get; }
    public ICommand AssignAppointmentCommand { get; }
    public ICommand UpdateAppointmentCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    private async void LoadAppointments()
    {
        var appointments = await _navstevaRepository.GetAppointmentsByDoctorId(_doctor.IdZamestnanec);
        AppointmentRequests.Clear();
        FutureAppointments.Clear();
        PastAppointments.Clear();

        if (appointments != null)
            foreach (var appointment in appointments)
            {
                var status = appointment.StatusId;

                var pacient = await _navstevaRepository.GetPatientNameByAppointmentId(appointment.IdNavsteva);

                var roomNumber = "";
                if (appointment.MistnostId.HasValue)
                {
                    var mistnost = await _mistnostRepository.GetMistnostById(appointment.MistnostId.Value);
                    if (mistnost != null) roomNumber = mistnost.Cislo.ToString();
                }

                var appointmentWithPatient = new
                {
                    appointment.IdNavsteva,
                    appointment.PacientId,
                    appointment.Datum,
                    appointment.MistnostId,
                    appointment.StatusId,
                    PACIENTJMENO = pacient.FirstName,
                    PACIENTPRIJMENI = pacient.LastName,
                    MISTNOST = roomNumber
                };

                if (status == 3)
                {
                    AppointmentRequests.Add(appointmentWithPatient);
                }
                else if (status == 1)
                {
                    var appointmentDate = appointment.Datum;

                    if (appointmentDate.HasValue)
                    {
                        if (appointmentDate.Value >= DateTime.Now)
                        
                            FutureAppointments.Add(appointmentWithPatient);

                        else
                        {
                            appointment.StatusId = 4;
                            _navstevaRepository.UpdateNavsteva(appointment);
                            PastAppointments.Add(appointmentWithPatient);
                        }
                    }
                    else
                    {
                        FutureAppointments.Add(appointmentWithPatient);
                    }
                } else if (status == 4)
                {
                    PastAppointments.Add(appointmentWithPatient);
                }
            }
    }

    public void SetDoctor(ZAMESTNANEC doctor)
    {
        _doctor = doctor;
        LoadAppointments();
    }
    private async void AcceptAppointment(object parameter)
    {
        if (parameter != null)
        {
            dynamic appointment = parameter;
            var navsteva = new NAVSTEVA
            {
                IdNavsteva = (int)appointment.IdNavsteva,
                PacientId = (int)appointment.PacientId,
                StatusId = appointment.StatusId,
                Datum = appointment.Datum,
                MistnostId = appointment.Mistnost
            };

            await _navstevaRepository.UpdateNavsteva(navsteva);
            LoadAppointments();
        }
    }

    private async void CancelAppointment(object parameter)
    {
        if (parameter != null)
        {
            dynamic appointment = parameter;
            var navsteva = new NAVSTEVA
            {
                IdNavsteva = appointment.IDNAVSTEVA,
                PacientId = appointment.PACIENTID,
                StatusId = 2
            };

            await _navstevaRepository.UpdateNavsteva(navsteva);
            LoadAppointments();
        }
    }

    private void AssignAppointment(object parameter)
    {
        if (parameter != null)
        {
            dynamic appointment = parameter;
            var navsteva = new NAVSTEVA
            {
                IdNavsteva = (int)appointment.IdNavsteva,
                PacientId = (int)appointment.PacientId,
                StatusId = (int)appointment.StatusId,
                Datum = appointment.Datum,
                MistnostId = appointment.MistnostId
            };

            _windowService.OpenAssignAppointmentWindow(navsteva, async updatedAppointment =>
            {
                if (updatedAppointment != null)
                {
                    await _navstevaRepository.UpdateNavsteva(updatedAppointment);
                    LoadAppointments();
                }
            });
        }
    }

    private void UpdateAppointment(object parameter)
    {
        if (parameter != null)
        {
            dynamic appointment = parameter;
            var navsteva = new NAVSTEVA
            {
                IdNavsteva = (int)appointment.IdNavsteva,
                PacientId = (int)appointment.PacientId,
                StatusId = (int)appointment.StatusId,
                Datum = appointment.Datum,
                MistnostId = appointment.MistnostId
            };

            _windowService.UpdateAppointmentWindow(navsteva, async updatedAppointment =>
            {
                if (updatedAppointment != null)
                {
                    await _navstevaRepository.UpdateNavsteva(updatedAppointment);
                    LoadAppointments();
                }
            }, _doctor.IdZamestnanec); // Pass the doctor ID here
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}