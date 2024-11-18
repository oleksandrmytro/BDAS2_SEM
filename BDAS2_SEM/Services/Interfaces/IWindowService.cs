using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Services.Interfaces
{
    public interface IWindowService
    {
        void OpenAdminWindow();
        void OpenAuthWindow();
        void OpenNewEmployeeWindow(UZIVATEL_DATA user, Action<bool> onClosed);
        void OpenNewPatientWindow(UZIVATEL_DATA userData, Action<bool> onClosed);
        void OpenAddAddressWindow(Action<ADRESA> onAddressAdded);
        void OpenAddPositionWindow(Action<POZICE> onPositionAdded);
        void OpenPatientWindow(PACIENT pacient);
        void OpenDoctorWindow(ZAMESTNANEC zamestnanec);
        void CloseWindow(Action closeAction);
    }
}
