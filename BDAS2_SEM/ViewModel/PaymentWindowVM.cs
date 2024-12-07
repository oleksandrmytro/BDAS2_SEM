// ViewModel/PaymentWindowVM.cs
using BDAS2_SEM.Commands;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class PaymentWindowVM : INotifyPropertyChanged
    {
        private decimal _amount;
        private string _selectedPaymentMethod;
        private string _cardNumber;
        private string _expiry;
        private string _cvv;
        private string _cashGiven;
        private string _change;
        private bool _isCardMethod;
        private bool _isCashMethod;

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                if (_selectedPaymentMethod != value)
                {
                    _selectedPaymentMethod = value;
                    OnPropertyChanged(nameof(SelectedPaymentMethod));
                    UpdateVisibility();
                }
            }
        }

        public string CardNumber
        {
            get => _cardNumber;
            set
            {
                _cardNumber = value;
                OnPropertyChanged(nameof(CardNumber));
            }
        }

        public string Expiry
        {
            get => _expiry;
            set
            {
                _expiry = value;
                OnPropertyChanged(nameof(Expiry));
            }
        }

        public string CVV
        {
            get => _cvv;
            set
            {
                _cvv = value;
                OnPropertyChanged(nameof(CVV));
            }
        }

        public string CashGiven
        {
            get => _cashGiven;
            set
            {
                _cashGiven = value;
                OnPropertyChanged(nameof(CashGiven));
                CalculateChange();
            }
        }

        public string Change
        {
            get => _change;
            set
            {
                _change = value;
                OnPropertyChanged(nameof(Change));
            }
        }

        public bool IsCardMethod
        {
            get => _isCardMethod;
            set
            {
                if (_isCardMethod != value)
                {
                    _isCardMethod = value;
                    OnPropertyChanged(nameof(IsCardMethod));
                }
            }
        }

        public bool IsCashMethod
        {
            get => _isCashMethod;
            set
            {
                if (_isCashMethod != value)
                {
                    _isCashMethod = value;
                    OnPropertyChanged(nameof(IsCashMethod));
                }
            }
        }

        public ICommand ConfirmPaymentCommand { get; }
        public ICommand CancelCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<bool?> CloseWindowEvent;

        public PaymentWindowVM(decimal amount)
        {
            Amount = amount;
            SelectedPaymentMethod = "karta"; // За замовчуванням
            ConfirmPaymentCommand = new RelayCommand(ConfirmPayment);
            CancelCommand = new RelayCommand(Cancel);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateVisibility()
        {
            if (SelectedPaymentMethod == "karta")
            {
                IsCardMethod = true;
                IsCashMethod = false;
            }
            else if (SelectedPaymentMethod == "hotovost")
            {
                IsCardMethod = false;
                IsCashMethod = true;
            }
            else
            {
                IsCardMethod = false;
                IsCashMethod = false;
            }
        }

        private void CalculateChange()
        {
            if (decimal.TryParse(CashGiven, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal cash))
            {
                if (cash >= Amount)
                {
                    decimal changeAmount = cash - Amount;
                    Change = $"{changeAmount:N2}"; // Формат валюти
                }
                else
                {
                    Change = "Недостатньо коштів";
                }
            }
            else
            {
                Change = string.Empty;
            }
        }

        private void ConfirmPayment(object parameter)
        {
            if (SelectedPaymentMethod == "karta")
            {
                // Перевірка введених даних картки
                if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(Expiry) || string.IsNullOrWhiteSpace(CVV))
                {
                    MessageBox.Show("Будь ласка, заповніть всі поля картки.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Додайте додаткову валідацію при необхідності
                CloseWindowEvent?.Invoke(this, true);
            }
            else if (SelectedPaymentMethod == "hotovost")
            {
                // Перевірка суми готівки
                if (decimal.TryParse(CashGiven, out decimal cashGiven))
                {
                    if (cashGiven < Amount)
                    {
                        MessageBox.Show("Сума готівки недостатня для оплати.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    CloseWindowEvent?.Invoke(this, true);
                }
                else
                {
                    MessageBox.Show("Введіть коректну суму готівки.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        private void Cancel(object parameter)
        {
            CloseWindowEvent?.Invoke(this, false);
        }
    }
}