using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class NAVSTEVA_DOCTOR_VIEW : INotifyPropertyChanged
    {
        private int navstevaId;
        private DateTime? visitDate;
        private string doctorFullName;
        private string navstevaStatus;
        private int pacientId;
        private int? mistnostId; // Новое свойство

        public int NavstevaId
        {
            get { return navstevaId; }
            set
            {
                if (navstevaId != value)
                {
                    navstevaId = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? VisitDate
        {
            get { return visitDate; }
            set
            {
                if (visitDate != value)
                {
                    visitDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DoctorFullName
        {
            get { return doctorFullName; }
            set
            {
                if (doctorFullName != value)
                {
                    doctorFullName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NavstevaStatus
        {
            get { return navstevaStatus; }
            set
            {
                if (navstevaStatus != value)
                {
                    navstevaStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PacientId
        {
            get { return pacientId; }
            set
            {
                if (pacientId != value)
                {
                    pacientId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? MistnostId // Новое свойство
        {
            get { return mistnostId; }
            set
            {
                if (mistnostId != value)
                {
                    mistnostId = value;
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