using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View;
using BDAS2_SEM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using BDAS2_SEM.View.AdminViews;
using BDAS2_SEM.View.PatientViews;
using BDAS2_SEM.View.DoctorViews;

namespace BDAS2_SEM.Services
{
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPatientContextService _patientContextService;

        public WindowService(IServiceProvider serviceProvider, IPatientContextService patientContextService)
        {
            _serviceProvider = serviceProvider;
            _patientContextService = patientContextService;
        }

        public void OpenEditPatientWindow(PACIENT patient)
        {
            var editPatientWindow = new EditPatientWindow();
            var pacientRepository = _serviceProvider.GetRequiredService<IPacientRepository>();
            var adresaRepository = _serviceProvider.GetRequiredService<IAdresaRepository>();
            var viewModel = new EditPatientVM(patient, pacientRepository, adresaRepository, this, (isSaved) =>
            {
                if (isSaved)
                {
                    // Действия после закрытия окна, если данные были сохранены
                }
            });
            editPatientWindow.DataContext = viewModel;
            editPatientWindow.ShowDialog();
        }

        public void OpenDoctorsListWindow()
        {
            var doctorsListView = _serviceProvider.GetRequiredService<DoctorsListView>();
            var doctorsListVM = _serviceProvider.GetRequiredService<DoctorsListVM>();
            doctorsListView.DataContext = doctorsListVM;
            doctorsListView.ShowDialog();
        }

        public void OpenAdminWindow()
        {
            var adminWindow = _serviceProvider.GetRequiredService<AdminWindow>();
            adminWindow.Show();

            CloseAuthWindow();
        }

        public void OpenAuthWindow()
        {
            var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();
        }

        public void OpenNewEmployeeWindow(UZIVATEL_DATA user, Action<bool> onClosed)
        {
            var newEmployeeWindow = new NewEmployeeWindow();
            var newEmployeeVM = new NewEmployeeVM(user, this, _serviceProvider.GetRequiredService<IServiceProvider>(), onClosed);
            newEmployeeWindow.DataContext = newEmployeeVM;
            newEmployeeWindow.ShowDialog();
        }

        public void OpenNewPatientWindow(UZIVATEL_DATA userData, Action<bool> onClosed)
        {
            var window = new NewPatientWindow();
            var viewModel = new NewPatientVM(userData, this, _serviceProvider, onClosed);
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        public void CloseWindow(Action closeAction)
        {
            closeAction?.Invoke();
        }

        public void OpenAssignAppointmentWindow(NAVSTEVA navsteva, Action<NAVSTEVA> onClose)
        {
            var viewModel = new AssignAppointmentVM(navsteva);
            var window = new AssignAppointmentView
            {
                DataContext = viewModel
            };
            viewModel.CloseAction = (updatedNavsteva) =>
            {
                onClose(updatedNavsteva);
                window.Close();
            };
            window.ShowDialog();
        }

        public void OpenAddAddressWindow(Action<ADRESA> onAddressAdded)
        {
            var addAddressWindow = new AddAddressWindow();
            var addAddressVM = new AddAddressVM(onAddressAdded, this, _serviceProvider);
            addAddressWindow.DataContext = addAddressVM;
            addAddressWindow.ShowDialog();
        }

        public void OpenDoctorWindow(ZAMESTNANEC zamestnanec) // Змінено тип параметра
        {
            var doctorViewModel = _serviceProvider.GetRequiredService<DoctorsVM>();
            doctorViewModel.SetEmployee(zamestnanec);
            var doctorWindow = new DoctorsWindow(doctorViewModel);
            doctorWindow.Show();
        }

        public void OpenAddPositionWindow(Action<POZICE> onPositionAdded)
        {
            var addPositionWindow = new AddPositionWindow();
            var addPositionVM = new AddPositionVM(onPositionAdded, this, _serviceProvider.GetRequiredService<IPoziceRepository>());
            addPositionWindow.DataContext = addPositionVM;
            addPositionWindow.ShowDialog();
        }

        public void OpenPatientWindow(PACIENT pacient)
        {
            var patientContextService = _serviceProvider.GetRequiredService<IPatientContextService>();
            patientContextService.CurrentPatient = pacient;

            var patientWindow = _serviceProvider.GetRequiredService<PatientsWindow>();
            var viewModel = _serviceProvider.GetRequiredService<PatientsVM>();
            patientWindow.DataContext = viewModel;
            patientWindow.Show();
        }


        private void CloseAuthWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is AuthWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}