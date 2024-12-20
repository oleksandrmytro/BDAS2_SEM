﻿using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View.PatientViews;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class PPlatbaVM : INotifyPropertyChanged
    {
        private readonly IPDiagnosesRepository _ipdDiagnosesRepository;
        private readonly IPatientContextService _patientContextService;
        private readonly IPlatbaRepository _platbaRepository;
        private readonly INavstevaRepository _navstevaRepository;

        private ObservableCollection<PDiagnosesDetail> _diagnoses;
        public ObservableCollection<PDiagnosesDetail> Diagnoses
        {
            get => _diagnoses;
            set
            {
                _diagnoses = value;
                OnPropertyChanged();
            }
        }

        private decimal _totalPayment;
        public decimal TotalPayment
        {
            get => _totalPayment;
            set
            {
                _totalPayment = value;
                OnPropertyChanged();
            }
        }

        public ICommand PayIndividualPaymentCommand { get; }
        public ICommand RefreshCommand { get; }

        public PPlatbaVM(IPDiagnosesRepository ipdDiagnosesRepository, IPatientContextService patientContextService, IPlatbaRepository platbaRepository, INavstevaRepository navstevaRepository)
        {
            Diagnoses = new ObservableCollection<PDiagnosesDetail>();
            _ipdDiagnosesRepository = ipdDiagnosesRepository;
            _patientContextService = patientContextService;
            _platbaRepository = platbaRepository;
            _navstevaRepository = navstevaRepository;

            PayIndividualPaymentCommand = new RelayCommand<PDiagnosesDetail>(async (diagnosis) => await ExecutePayIndividualPaymentAsync(diagnosis), CanExecutePayIndividualPayment);
            RefreshCommand = new AsyncRelayCommand(LoadDiagnosesAsync);

            LoadDiagnosesAsync();
        }

        private bool CanExecutePayIndividualPayment(PDiagnosesDetail diagnosis)
        {
            return diagnosis != null && diagnosis.StatusIdStatus != 2;
        }

        private async Task ExecutePayIndividualPaymentAsync(PDiagnosesDetail diagnosis)
        {
            if (diagnosis == null || diagnosis.StatusIdStatus == 2)
                return;

            var paymentWindow = new PaymentWindow(diagnosis.TotalCost);
            var paymentWindowVM = (PaymentWindowVM)paymentWindow.DataContext;

            bool? paymentResult = paymentWindow.ShowDialog();

            if (paymentResult == true)
            {
                try
                {
                    string typPlatby = paymentWindowVM.SelectedPaymentMethod;
                    long? cisloKarty = null;
                    decimal? prijato = null;
                    decimal? vraceno = null;

                    if (typPlatby == "karta")
                    {
                        cisloKarty = long.Parse(paymentWindowVM.CardNumber);
                    }
                    else if (typPlatby == "hotovost")
                    {
                        if (decimal.TryParse(paymentWindowVM.CashGiven, out decimal cashGiven))
                        {
                            prijato = cashGiven;
                            vraceno = cashGiven - diagnosis.TotalCost;
                        }
                    }

                    int platbaId = await _platbaRepository.ManagePaymentAsync(
                        action: "INSERT",
                        idPlatba: null,
                        castka: diagnosis.TotalCost,
                        datum: DateTime.Now,
                        typPlatby: typPlatby,
                        navstevaId: diagnosis.IdNavsteva,
                        cisloKarty: cisloKarty,
                        prijato: prijato,
                        vraceno: vraceno
                    );

                    if (platbaId > 0)
                    {
                        NAVSTEVA navsteva = await _navstevaRepository.GetNavstevaById(diagnosis.IdNavsteva);
                        navsteva.StatusId = 5;
                        await _navstevaRepository.UpdateNavsteva(navsteva);
                        Diagnoses.Remove(diagnosis);
                        // Перерахунок загальної суми
                        TotalPayment = Diagnoses.Where(d => d.StatusIdStatus != 2).Sum(d => d.TotalCost);

                        MessageBox.Show($"Payment for a diagnosis '{diagnosis.DiagnosisName}' successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"An error occurred when paying for a diagnosis '{diagnosis.DiagnosisName}'. Try again.",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
            }
        }

        private async Task LoadDiagnosesAsync(object parameter = null)
        {
            var currentPatient = _patientContextService.CurrentPatient;
            if (currentPatient == null)
            {
                Diagnoses = new ObservableCollection<PDiagnosesDetail>();
                TotalPayment = 0;
                return;
            }

            var pacientId = currentPatient.IdPacient;
            var diagnosesList = await _ipdDiagnosesRepository.GetPDiagnosesByPatientIdAsync(pacientId);
            Diagnoses.Clear();
            foreach (var diagnosis in diagnosesList)
            {
                Diagnoses.Add(diagnosis);
            }

            TotalPayment = Diagnoses.Where(d => d.StatusIdStatus != 2).Sum(d => d.TotalCost);

            OnPropertyChanged(nameof(Diagnoses));
            OnPropertyChanged(nameof(TotalPayment));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}