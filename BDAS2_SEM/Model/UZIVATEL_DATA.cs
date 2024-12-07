using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Model
{
    public class UZIVATEL_DATA : INotifyPropertyChanged
    {
        private int id;
        private string email;
        private string heslo;
        private int roleId;
        public int? pacientId { get; set; }
        public int? zamestnanecId { get; set; }

        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Heslo
        {
            get { return heslo; }
            set
            {
                if (heslo != value)
                {
                    heslo = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RoleId
        {
            get { return roleId; }
            set
            {
                if (roleId != value)
                {
                    roleId = value;
                    OnPropertyChanged();
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
