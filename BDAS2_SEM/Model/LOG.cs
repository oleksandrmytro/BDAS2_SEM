using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Model
{
    public class LOG : INotifyPropertyChanged
    {
        public int idLog;
        public string tableName;
        public string operationType;
        public DateTime operationData;
        public string? oldValues;
        public string? newValues;

        public int IdLog
        {
            get { return idLog; }
            set
            {
                if (idLog != value)
                {
                    idLog = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime OperationData
        {
            get { return operationData; }
            set
            {
                if (operationData != value)
                {
                    operationData = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TableName
        {
            get { return tableName; }
            set
            {
                if (tableName != value)
                {
                    tableName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OperationType
        {
            get { return operationType; }
            set
            {
                if (operationType != value)
                {
                    operationType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? OldValues
        {
            get { return oldValues; }
            set
            {
                if (oldValues != value)
                {
                    oldValues = value;
                    OnPropertyChanged();
                }
            }
        }
        public string? NewValues
        {
            get { return newValues; }
            set
            {
                if (newValues != value)
                {
                    newValues = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

}
