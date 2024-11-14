using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View;
using BDAS2_SEM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace BDAS2_SEM.Services
{
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenAdminWindow()
        {
            var adminWindow = _serviceProvider.GetRequiredService<AdminWindow>();
            adminWindow.Show();

            CloseAuthWindow();
        }

        public void OpenNewEmployeeWindow(UZIVATEL_DATA user, Action<bool> onClosed)
        {
            var newEmployeeWindow = new NewEmployeeWindow();
            var newEmployeeVM = new NewEmployeeVM(user, this, _serviceProvider.GetRequiredService<IServiceProvider>(), onClosed);
            newEmployeeWindow.DataContext = newEmployeeVM;
            newEmployeeWindow.ShowDialog();
        }

        public void CloseWindow(Action closeAction)
        {
            closeAction?.Invoke();
        }

        public void OpenAddAddressWindow(Action<ADRESA> onAddressAdded)
        {
            var addAddressWindow = new AddAddressWindow();
            var addAddressVM = new AddAddressVM(onAddressAdded, this, _serviceProvider);
            addAddressWindow.DataContext = addAddressVM;
            addAddressWindow.ShowDialog();
        }

        public void OpenAddPositionWindow(Action<POZICE> onPositionAdded)
        {
            var addPositionWindow = new AddPositionWindow();
            var addPositionVM = new AddPositionVM(onPositionAdded, this, _serviceProvider.GetRequiredService<IPoziceRepository>());
            addPositionWindow.DataContext = addPositionVM;
            addPositionWindow.ShowDialog();
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