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
        void OpenNewEmployeeWindow(UZIVATEL_DATA user);
        void OpenAddAddressWindow(Action<ADRESA> onAddressAdded);
        void OpenAddPositionWindow(Action<POZICE> onPositionAdded);
        void CloseWindow(Action closeAction);
    }
}
