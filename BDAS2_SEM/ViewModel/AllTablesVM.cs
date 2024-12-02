// Updated AllTablesVM with PascalCase Properties Matching XAML Bindings

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.View.AdminViews;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.ViewModel
{
    public class AllTablesVM : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAdresaRepository _adresaRepository;
        private readonly IBlobTableRepository _blobTableRepository;
        private readonly IDiagnozaRepository _diagnozaRepository;
        private readonly IHotovostRepository _hotovostRepository;
        private readonly IKartaRepository _kartaRepository;
        private readonly ILekDiagnozaRepository _lekDiagnozaRepository;
        private readonly ILekRepository _lekRepository;
        private readonly INavstevaDiagnozaRepository _navstevaDiagnozaRepository;
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IOperaceRepository _operaceRepository;
        private readonly IOperaceZamestnanecRepository _operaceZamestnanecRepository;
        private readonly IOrdinaceRepository _ordinaceRepository;
        private readonly IOrdinaceZamestnanecRepository _ordinaceZamestnanecRepository;
        private readonly IPacientRepository _pacientRepository;
        private readonly IPlatbaRepository _platbaRepository;
        private readonly IPoziceRepository _poziceRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IZamestnanecNavstevaRepository _zamestnanecNavstevaRepository;
        private readonly IZamestnanecRepository _zamestnanecRepository;

        public ICommand EditCommand { get; }

        // Renamed properties to PascalCase to match XAML bindings
        public ObservableCollection<ADRESA> Adresa { get; set; } = new ObservableCollection<ADRESA>();
        public ObservableCollection<BLOB_TABLE> BlobTable { get; set; } = new ObservableCollection<BLOB_TABLE>();
        public ObservableCollection<DIAGNOZA> Diagnosis { get; set; } = new ObservableCollection<DIAGNOZA>();
        public ObservableCollection<HOTOVOST> Hotovost { get; set; } = new ObservableCollection<HOTOVOST>();
        public ObservableCollection<KARTA> Karta { get; set; } = new ObservableCollection<KARTA>();
        public ObservableCollection<LEK_DIAGNOZA> LekDiagnoza { get; set; } = new ObservableCollection<LEK_DIAGNOZA>();
        public ObservableCollection<LEK> Lek { get; set; } = new ObservableCollection<LEK>();
        public ObservableCollection<NAVSTEVA_DIAGNOZA> NavstevaDiagnoza { get; set; } = new ObservableCollection<NAVSTEVA_DIAGNOZA>();
        public ObservableCollection<NAVSTEVA> Navsteva { get; set; } = new ObservableCollection<NAVSTEVA>();
        public ObservableCollection<OPERACE> Operace { get; set; } = new ObservableCollection<OPERACE>();
        public ObservableCollection<OPERACE_ZAMESTNANEC> OperaceZamestnanec { get; set; } = new ObservableCollection<OPERACE_ZAMESTNANEC>();
        public ObservableCollection<ORDINACE> Ordinace { get; set; } = new ObservableCollection<ORDINACE>();
        public ObservableCollection<ORDINACE_ZAMESTNANEC> OrdinaceZamestnanec { get; set; } = new ObservableCollection<ORDINACE_ZAMESTNANEC>();
        public ObservableCollection<PACIENT> Pacient { get; set; } = new ObservableCollection<PACIENT>();
        public ObservableCollection<PLATBA> Platba { get; set; } = new ObservableCollection<PLATBA>();
        public ObservableCollection<POZICE> Pozice { get; set; } = new ObservableCollection<POZICE>();
        public ObservableCollection<UZIVATEL_DATA> UzivatelData { get; set; } = new ObservableCollection<UZIVATEL_DATA>();
        public ObservableCollection<ZAMESTNANEC_NAVSTEVA> ZamestnanecNavsteva { get; set; } = new ObservableCollection<ZAMESTNANEC_NAVSTEVA>();
        public ObservableCollection<ZAMESTNANEC> Zamestnanec { get; set; } = new ObservableCollection<ZAMESTNANEC>();

        public AllTablesVM(
            IServiceProvider serviceProvider,
            IAdresaRepository adresaRepository,
            IBlobTableRepository blobTableRepository,
            IDiagnozaRepository diagnozaRepository,
            IHotovostRepository hotovostRepository,
            IKartaRepository kartaRepository,
            ILekDiagnozaRepository lekDiagnozaRepository,
            ILekRepository lekRepository,
            INavstevaDiagnozaRepository navstevaDiagnozaRepository,
            INavstevaRepository navstevaRepository,
            IOperaceRepository operaceRepository,
            IOperaceZamestnanecRepository operaceZamestnanecRepository,
            IOrdinaceRepository ordinaceRepository,
            IOrdinaceZamestnanecRepository ordinaceZamestnanecRepository,
            IPacientRepository pacientRepository,
            IPlatbaRepository platbaRepository,
            IPoziceRepository poziceRepository,
            IUzivatelDataRepository uzivatelDataRepository,
            IZamestnanecNavstevaRepository zamestnanecNavstevaRepository,
            IZamestnanecRepository zamestnanecRepository)
        {
            _serviceProvider = serviceProvider;
            _adresaRepository = adresaRepository;
            _blobTableRepository = blobTableRepository;
            _diagnozaRepository = diagnozaRepository;
            _hotovostRepository = hotovostRepository;
            _kartaRepository = kartaRepository;
            _lekDiagnozaRepository = lekDiagnozaRepository;
            _lekRepository = lekRepository;
            _navstevaDiagnozaRepository = navstevaDiagnozaRepository;
            _navstevaRepository = navstevaRepository;
            _operaceRepository = operaceRepository;
            _operaceZamestnanecRepository = operaceZamestnanecRepository;
            _ordinaceRepository = ordinaceRepository;
            _ordinaceZamestnanecRepository = ordinaceZamestnanecRepository;
            _pacientRepository = pacientRepository;
            _platbaRepository = platbaRepository;
            _poziceRepository = poziceRepository;
            _uzivatelDataRepository = uzivatelDataRepository;
            _zamestnanecNavstevaRepository = zamestnanecNavstevaRepository;
            _zamestnanecRepository = zamestnanecRepository;

            EditCommand = new RelayCommand(EditItem);
            // Initiate data loading asynchronously
            LoadDataAsync();
        }

        private void EditItem(object parameter)
        {
            if (parameter == null) return;

            // Open the EditWindow with the selected item
            var window = new EditWindow(parameter, _serviceProvider);
            if (window.ShowDialog() == true)
            {
                // Refresh data after successful edit
                LoadDataAsync();
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                Adresa = new ObservableCollection<ADRESA>(await _adresaRepository.GetAllAddresses());
                OnPropertyChanged(nameof(Adresa));

                BlobTable = new ObservableCollection<BLOB_TABLE>(await _blobTableRepository.GetAllBlobTables());
                OnPropertyChanged(nameof(BlobTable));

                Diagnosis = new ObservableCollection<DIAGNOZA>(await _diagnozaRepository.GetAllDiagnoses());
                OnPropertyChanged(nameof(Diagnosis));

                Hotovost = new ObservableCollection<HOTOVOST>(await _hotovostRepository.GetAllHotovost());
                OnPropertyChanged(nameof(Hotovost));

                Karta = new ObservableCollection<KARTA>(await _kartaRepository.GetAllKarta());
                OnPropertyChanged(nameof(Karta));

                LekDiagnoza = new ObservableCollection<LEK_DIAGNOZA>(await _lekDiagnozaRepository.GetAllLekDiagnoza());
                OnPropertyChanged(nameof(LekDiagnoza));

                Lek = new ObservableCollection<LEK>(await _lekRepository.GetAllLeks());
                OnPropertyChanged(nameof(Lek));

                NavstevaDiagnoza = new ObservableCollection<NAVSTEVA_DIAGNOZA>(await _navstevaDiagnozaRepository.GetAllNavstevaDiagnozas());
                OnPropertyChanged(nameof(NavstevaDiagnoza));

                Navsteva = new ObservableCollection<NAVSTEVA>(await _navstevaRepository.GetAllNavstevas());
                OnPropertyChanged(nameof(Navsteva));

                Operace = new ObservableCollection<OPERACE>(await _operaceRepository.GetAllOperaces());
                OnPropertyChanged(nameof(Operace));

                OperaceZamestnanec = new ObservableCollection<OPERACE_ZAMESTNANEC>(await _operaceZamestnanecRepository.GetAllOperaceZamestnanecs());
                OnPropertyChanged(nameof(OperaceZamestnanec));

                Ordinace = new ObservableCollection<ORDINACE>(await _ordinaceRepository.GetAllOrdinaces());
                OnPropertyChanged(nameof(Ordinace));

                OrdinaceZamestnanec = new ObservableCollection<ORDINACE_ZAMESTNANEC>(await _ordinaceZamestnanecRepository.GetAllOrdinaceZamestnanecs());
                OnPropertyChanged(nameof(OrdinaceZamestnanec));

                Pacient = new ObservableCollection<PACIENT>(await _pacientRepository.GetAllPacients());
                OnPropertyChanged(nameof(Pacient));

                Platba = new ObservableCollection<PLATBA>(await _platbaRepository.GetAllPlatbas());
                OnPropertyChanged(nameof(Platba));

                Pozice = new ObservableCollection<POZICE>(await _poziceRepository.GetAllPozices());
                OnPropertyChanged(nameof(Pozice));

                UzivatelData = new ObservableCollection<UZIVATEL_DATA>(await _uzivatelDataRepository.GetAllUzivatelDatas());
                OnPropertyChanged(nameof(UzivatelData));

                ZamestnanecNavsteva = new ObservableCollection<ZAMESTNANEC_NAVSTEVA>(await _zamestnanecNavstevaRepository.GetAllZamestnanecNavstevas());
                OnPropertyChanged(nameof(ZamestnanecNavsteva));

                Zamestnanec = new ObservableCollection<ZAMESTNANEC>(await _zamestnanecRepository.GetAllZamestnanci());
                OnPropertyChanged(nameof(Zamestnanec));
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user)
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