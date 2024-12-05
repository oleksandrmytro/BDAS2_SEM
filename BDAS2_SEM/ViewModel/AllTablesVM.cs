using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
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
        private readonly ILogRepository _logrepository;
        private readonly INavstevaDiagnozaRepository _navstevaDiagnozaRepository;
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IOperaceRepository _operaceRepository;
        private readonly IOperaceZamestnanecRepository _operaceZamestnanecRepository;
        private readonly IOrdinaceRepository _ordinaceRepository;
        private readonly IOrdinaceZamestnanecRepository _ordinaceZamestnanecRepository;
        private readonly IPacientRepository _pacientRepository;
        private readonly IPlatbaRepository _platbaRepository;
        private readonly IPoziceRepository _poziceRepository;
        private readonly IPriponaRepository _priponaRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ITypLekRepository _typLekRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IZamestnanecNavstevaRepository _zamestnanecNavstevaRepository;
        private readonly IZamestnanecRepository _zamestnanecRepository;

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand InsertCommand { get; }

        public ObservableCollection<ADRESA> Adresa { get; set; } = new ObservableCollection<ADRESA>();
        public ObservableCollection<BLOB_TABLE> BlobTable { get; set; } = new ObservableCollection<BLOB_TABLE>();
        public ObservableCollection<DIAGNOZA> Diagnosis { get; set; } = new ObservableCollection<DIAGNOZA>();
        public ObservableCollection<HOTOVOST> Hotovost { get; set; } = new ObservableCollection<HOTOVOST>();
        public ObservableCollection<KARTA> Karta { get; set; } = new ObservableCollection<KARTA>();
        public ObservableCollection<LEK_DIAGNOZA> LekDiagnoza { get; set; } = new ObservableCollection<LEK_DIAGNOZA>();
        public ObservableCollection<LEK> Lek { get; set; } = new ObservableCollection<LEK>();
        public ObservableCollection<LOG> Log { get; set; } = new ObservableCollection<LOG>();
        public ObservableCollection<NAVSTEVA_DIAGNOZA> NavstevaDiagnoza { get; set; } = new ObservableCollection<NAVSTEVA_DIAGNOZA>();
        public ObservableCollection<NAVSTEVA> Navsteva { get; set; } = new ObservableCollection<NAVSTEVA>();
        public ObservableCollection<OPERACE> Operace { get; set; } = new ObservableCollection<OPERACE>();
        public ObservableCollection<OPERACE_ZAMESTNANEC> OperaceZamestnanec { get; set; } = new ObservableCollection<OPERACE_ZAMESTNANEC>();
        public ObservableCollection<ORDINACE> Ordinace { get; set; } = new ObservableCollection<ORDINACE>();
        public ObservableCollection<ORDINACE_ZAMESTNANEC> OrdinaceZamestnanec { get; set; } = new ObservableCollection<ORDINACE_ZAMESTNANEC>();
        public ObservableCollection<PACIENT> Pacient { get; set; } = new ObservableCollection<PACIENT>();
        public ObservableCollection<PLATBA> Platba { get; set; } = new ObservableCollection<PLATBA>();
        public ObservableCollection<POZICE> Pozice { get; set; } = new ObservableCollection<POZICE>();
        public ObservableCollection<PRIPONA> Pripona { get; set; } = new ObservableCollection<PRIPONA>();
        public ObservableCollection<STATUS> Status { get; set; } = new ObservableCollection<STATUS>();
        public ObservableCollection<TYP_LEK> TypLek { get; set; } = new ObservableCollection<TYP_LEK>();
        public ObservableCollection<UZIVATEL_DATA> UzivatelData { get; set; } = new ObservableCollection<UZIVATEL_DATA>();
        public ObservableCollection<ZAMESTNANEC_NAVSTEVA> ZamestnanecNavsteva { get; set; } = new ObservableCollection<ZAMESTNANEC_NAVSTEVA>();
        public ObservableCollection<ZAMESTNANEC> Zamestnanec { get; set; } = new ObservableCollection<ZAMESTNANEC>();

        // Пошукові властивості
        private string _adresaSearchText;
        public string AdresaSearchText
        {
            get => _adresaSearchText;
            set
            {
                if (_adresaSearchText != value)
                {
                    _adresaSearchText = value;
                    OnPropertyChanged();
                    AdresaView.Refresh();
                }
            }
        }

        private string _zamestnanecSearchText;
        public string ZamestnanecSearchText
        {
            get => _zamestnanecSearchText;
            set
            {
                if (_zamestnanecSearchText != value)
                {
                    _zamestnanecSearchText = value;
                    OnPropertyChanged();
                    ZamestnanecView.Refresh();
                }
            }
        }

        private string _diagnózaSearchText;
        public string DiagnózaSearchText
        {
            get => _diagnózaSearchText;
            set
            {
                if (_diagnózaSearchText != value)
                {
                    _diagnózaSearchText = value;
                    OnPropertyChanged();
                    DiagnózaView.Refresh();
                }
            }
        }

        private string _ordinaceSearchText;
        public string OrdinaceSearchText
        {
            get => _ordinaceSearchText;
            set
            {
                if (_ordinaceSearchText != value)
                {
                    _ordinaceSearchText = value;
                    OnPropertyChanged();
                    OrdinaceView.Refresh();
                }
            }
        }

        private string _uzivatelskaDataSearchText;
        public string UzivatelskaDataSearchText
        {
            get => _uzivatelskaDataSearchText;
            set
            {
                if (_uzivatelskaDataSearchText != value)
                {
                    _uzivatelskaDataSearchText = value;
                    OnPropertyChanged();
                    UzivatelskaDataView.Refresh();
                }
            }
        }

        private string _platbaSearchText;
        public string PlatbaSearchText
        {
            get => _platbaSearchText;
            set
            {
                if (_platbaSearchText != value)
                {
                    _platbaSearchText = value;
                    OnPropertyChanged();
                    PlatbaView.Refresh();
                }
            }
        }

        private string _pacientSearchText;
        public string PacientSearchText
        {
            get => _pacientSearchText;
            set
            {
                if (_pacientSearchText != value)
                {
                    _pacientSearchText = value;
                    OnPropertyChanged();
                    PacientView.Refresh();
                }
            }
        }

        private string _poziceSearchText;
        public string PoziceSearchText
        {
            get => _poziceSearchText;
            set
            {
                if (_poziceSearchText != value)
                {
                    _poziceSearchText = value;
                    OnPropertyChanged();
                    PoziceView.Refresh();
                }
            }
        }

        private string _priponaSearchText;
        public string PriponaSearchText
        {
            get => _priponaSearchText;
            set
            {
                if (_priponaSearchText != value)
                {
                    _priponaSearchText = value;
                    OnPropertyChanged();
                    PriponaView.Refresh();
                }
            }
        }

        private string _statusSearchText;
        public string StatusSearchText
        {
            get => _statusSearchText;
            set
            {
                if (_statusSearchText != value)
                {
                    _statusSearchText = value;
                    OnPropertyChanged();
                    StatusView.Refresh();
                }
            }
        }

        private string _typLekSearchText;
        public string TypLekSearchText
        {
            get => _typLekSearchText;
            set
            {
                if (_typLekSearchText != value)
                {
                    _typLekSearchText = value;
                    OnPropertyChanged();
                    TypLekView.Refresh();
                }
            }
        }

        private string _lekSearchText;
        public string LekSearchText
        {
            get => _lekSearchText;
            set
            {
                if (_lekSearchText != value)
                {
                    _lekSearchText = value;
                    OnPropertyChanged();
                    LekView.Refresh();
                }
            }
        }

        private string _operaceSearchText;
        public string OperaceSearchText
        {
            get => _operaceSearchText;
            set
            {
                if (_operaceSearchText != value)
                {
                    _operaceSearchText = value;
                    OnPropertyChanged();
                    OperaceView.Refresh();
                }
            }
        }

        private string _navstevaSearchText;
        public string NavstevaSearchText
        {
            get => _navstevaSearchText;
            set
            {
                if (_navstevaSearchText != value)
                {
                    _navstevaSearchText = value;
                    OnPropertyChanged();
                    NavstevaView.Refresh();
                }
            }
        }

        private string _operaceZamestnanecSearchText;
        public string OperaceZamestnanecSearchText
        {
            get => _operaceZamestnanecSearchText;
            set
            {
                if (_operaceZamestnanecSearchText != value)
                {
                    _operaceZamestnanecSearchText = value;
                    OnPropertyChanged();
                    OperaceZamestnanecView.Refresh();
                }
            }
        }

        private string _blobTableSearchText;
        public string BlobTableSearchText
        {
            get => _blobTableSearchText;
            set
            {
                if (_blobTableSearchText != value)
                {
                    _blobTableSearchText = value;
                    OnPropertyChanged();
                    BlobTableView.Refresh();
                }
            }
        }

        private string _ordinaceZamestnanecSearchText;
        public string OrdinaceZamestnanecSearchText
        {
            get => _ordinaceZamestnanecSearchText;
            set
            {
                if (_ordinaceZamestnanecSearchText != value)
                {
                    _ordinaceZamestnanecSearchText = value;
                    OnPropertyChanged();
                    OrdinaceZamestnanecView.Refresh();
                }
            }
        }

        private string _zamestnanecNavstevaSearchText;
        public string ZamestnanecNavstevaSearchText
        {
            get => _zamestnanecNavstevaSearchText;
            set
            {
                if (_zamestnanecNavstevaSearchText != value)
                {
                    _zamestnanecNavstevaSearchText = value;
                    OnPropertyChanged();
                    ZamestnanecNavstevaView.Refresh();
                }
            }
        }

        private string _navstevaDiagnozaSearchText;
        public string NavstevaDiagnozaSearchText
        {
            get => _navstevaDiagnozaSearchText;
            set
            {
                if (_navstevaDiagnozaSearchText != value)
                {
                    _navstevaDiagnozaSearchText = value;
                    OnPropertyChanged();
                    NavstevaDiagnozaView.Refresh();
                }
            }
        }

        private readonly ICollectionView _adresaView;
        public ICollectionView AdresaView => _adresaView;

        private readonly ICollectionView _zamestnanecView;
        public ICollectionView ZamestnanecView => _zamestnanecView;

        private readonly ICollectionView _diagnózaView;
        public ICollectionView DiagnózaView => _diagnózaView;

        private readonly ICollectionView _ordinaceView;
        public ICollectionView OrdinaceView => _ordinaceView;

        private readonly ICollectionView _uzivatelskaDataView;
        public ICollectionView UzivatelskaDataView => _uzivatelskaDataView;

        private readonly ICollectionView _platbaView;
        public ICollectionView PlatbaView => _platbaView;

        private readonly ICollectionView _pacientView;
        public ICollectionView PacientView => _pacientView;

        private readonly ICollectionView _poziceView;
        public ICollectionView PoziceView => _poziceView;

        private readonly ICollectionView _priponaView;
        public ICollectionView PriponaView => _priponaView;

        private readonly ICollectionView _statusView;
        public ICollectionView StatusView => _statusView;

        private readonly ICollectionView _typLekView;
        public ICollectionView TypLekView => _typLekView;

        private readonly ICollectionView _lekView;
        public ICollectionView LekView => _lekView;

        private readonly ICollectionView _operaceView;
        public ICollectionView OperaceView => _operaceView;

        private readonly ICollectionView _navstevaView;
        public ICollectionView NavstevaView => _navstevaView;

        private readonly ICollectionView _operaceZamestnanecView;
        public ICollectionView OperaceZamestnanecView => _operaceZamestnanecView;

        private readonly ICollectionView _blobTableView;
        public ICollectionView BlobTableView => _blobTableView;

        private readonly ICollectionView _ordinaceZamestnanecView;
        public ICollectionView OrdinaceZamestnanecView => _ordinaceZamestnanecView;

        private readonly ICollectionView _zamestnanecNavstevaView;
        public ICollectionView ZamestnanecNavstevaView => _zamestnanecNavstevaView;

        private readonly ICollectionView _navstevaDiagnozaView;
        public ICollectionView NavstevaDiagnozaView => _navstevaDiagnozaView;

        public AllTablesVM(
            IServiceProvider serviceProvider,
            IAdresaRepository adresaRepository,
            IBlobTableRepository blobTableRepository,
            IDiagnozaRepository diagnozaRepository,
            IHotovostRepository hotovostRepository,
            IKartaRepository kartaRepository,
            ILekDiagnozaRepository lekDiagnozaRepository,
            ILekRepository lekRepository,
            ILogRepository logRepository,
            INavstevaDiagnozaRepository navstevaDiagnozaRepository,
            INavstevaRepository navstevaRepository,
            IOperaceRepository operaceRepository,
            IOperaceZamestnanecRepository operaceZamestnanecRepository,
            IOrdinaceRepository ordinaceRepository,
            IOrdinaceZamestnanecRepository ordinaceZamestnanecRepository,
            IPacientRepository pacientRepository,
            IPlatbaRepository platbaRepository,
            IPoziceRepository poziceRepository,
            IPriponaRepository priponaRepository,
            IStatusRepository statusRepository,
            ITypLekRepository typLekRepository,
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
            _logrepository = logRepository;
            _navstevaDiagnozaRepository = navstevaDiagnozaRepository;
            _navstevaRepository = navstevaRepository;
            _operaceRepository = operaceRepository;
            _operaceZamestnanecRepository = operaceZamestnanecRepository;
            _ordinaceRepository = ordinaceRepository;
            _ordinaceZamestnanecRepository = ordinaceZamestnanecRepository;
            _pacientRepository = pacientRepository;
            _platbaRepository = platbaRepository;
            _poziceRepository = poziceRepository;
            _priponaRepository = priponaRepository;
            _statusRepository = statusRepository;
            _typLekRepository = typLekRepository;
            _uzivatelDataRepository = uzivatelDataRepository;
            _zamestnanecNavstevaRepository = zamestnanecNavstevaRepository;
            _zamestnanecRepository = zamestnanecRepository;

            EditCommand = new RelayCommand(EditItem);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            InsertCommand = new RelayCommand<Type>(ExecuteInsert);

            _adresaView = CollectionViewSource.GetDefaultView(Adresa);
            _adresaView.Filter = AdresaFilter;

            _zamestnanecView = CollectionViewSource.GetDefaultView(Zamestnanec);
            _zamestnanecView.Filter = ZamestnanecFilter;

            _diagnózaView = CollectionViewSource.GetDefaultView(Diagnosis);
            _diagnózaView.Filter = DiagnózaFilter;

            _ordinaceView = CollectionViewSource.GetDefaultView(Ordinace);
            _ordinaceView.Filter = OrdinaceFilter;

            _uzivatelskaDataView = CollectionViewSource.GetDefaultView(UzivatelData);
            _uzivatelskaDataView.Filter = UzivatelskaDataFilter;

            _platbaView = CollectionViewSource.GetDefaultView(Platba);
            _platbaView.Filter = PlatbaFilter;

            _pacientView = CollectionViewSource.GetDefaultView(Pacient);
            _pacientView.Filter = PacientFilter;

            _poziceView = CollectionViewSource.GetDefaultView(Pozice);
            _poziceView.Filter = PoziceFilter;

            _priponaView = CollectionViewSource.GetDefaultView(Pripona);
            _priponaView.Filter = PriponaFilter;

            _statusView = CollectionViewSource.GetDefaultView(Status);
            _statusView.Filter = StatusFilter;

            _typLekView = CollectionViewSource.GetDefaultView(TypLek);
            _typLekView.Filter = TypLekFilter;

            _lekView = CollectionViewSource.GetDefaultView(Lek);
            _lekView.Filter = LekFilter;

            _operaceView = CollectionViewSource.GetDefaultView(Operace);
            _operaceView.Filter = OperaceFilter;

            _navstevaView = CollectionViewSource.GetDefaultView(Navsteva);
            _navstevaView.Filter = NavstevaFilter;

            _operaceZamestnanecView = CollectionViewSource.GetDefaultView(OperaceZamestnanec);
            _operaceZamestnanecView.Filter = OperaceZamestnanecFilter;

            _blobTableView = CollectionViewSource.GetDefaultView(BlobTable);
            _blobTableView.Filter = BlobTableFilter;

            _ordinaceZamestnanecView = CollectionViewSource.GetDefaultView(OrdinaceZamestnanec);
            _ordinaceZamestnanecView.Filter = OrdinaceZamestnanecFilter;

            _zamestnanecNavstevaView = CollectionViewSource.GetDefaultView(ZamestnanecNavsteva);
            _zamestnanecNavstevaView.Filter = ZamestnanecNavstevaFilter;

            _navstevaDiagnozaView = CollectionViewSource.GetDefaultView(NavstevaDiagnoza);
            _navstevaDiagnozaView.Filter = NavstevaDiagnozaFilter;

            LoadDataAsync();
        }

        private void ExecuteInsert(Type modelType)
        {
            if (modelType == null) return;

            object newItem = Activator.CreateInstance(modelType);

            var window = new EditWindow(newItem, _serviceProvider);
            if (window.ShowDialog() == true)
            {
                switch (newItem)
                {
                    case ADRESA adresa:
                        Adresa.Add(adresa);
                        break;

                    case BLOB_TABLE blobTable:
                        BlobTable.Add(blobTable);
                        break;

                    case DIAGNOZA diagnoza:
                        Diagnosis.Add(diagnoza);
                        break;

                    case HOTOVOST hotovost:
                        Hotovost.Add(hotovost);
                        break;

                    case KARTA karta:
                        Karta.Add(karta);
                        break;

                    case LEK_DIAGNOZA lekDiagnoza:
                        LekDiagnoza.Add(lekDiagnoza);
                        break;

                    case LEK lek:
                        Lek.Add(lek);
                        break;

                    case LOG log:
                        Log.Add(log);
                        break;

                    case NAVSTEVA_DIAGNOZA navstevaDiagnoza:
                        NavstevaDiagnoza.Add(navstevaDiagnoza);
                        break;

                    case NAVSTEVA navsteva:
                        Navsteva.Add(navsteva);
                        break;

                    case OPERACE operace:
                        Operace.Add(operace);
                        break;

                    case OPERACE_ZAMESTNANEC operaceZamestnanec:
                        OperaceZamestnanec.Add(operaceZamestnanec);
                        break;

                    case ORDINACE ordinace:
                        Ordinace.Add(ordinace);
                        break;

                    case ORDINACE_ZAMESTNANEC ordinaceZamestnanec:
                        OrdinaceZamestnanec.Add(ordinaceZamestnanec);
                        break;

                    case PACIENT pacient:
                        Pacient.Add(pacient);
                        break;

                    case PLATBA platba:
                        Platba.Add(platba);
                        break;

                    case PRIPONA pripona:
                        Pripona.Add(pripona);
                        break;

                    case POZICE pozice:
                        Pozice.Add(pozice);
                        break;

                    case STATUS status:
                        Status.Add(status);
                        break;

                    case TYP_LEK typLek:
                        TypLek.Add(typLek);
                        break;

                    case UZIVATEL_DATA uzivatelData:
                        UzivatelData.Add(uzivatelData);
                        break;

                    case ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva:
                        ZamestnanecNavsteva.Add(zamestnanecNavsteva);
                        break;

                    case ZAMESTNANEC zamestnanec:
                        Zamestnanec.Add(zamestnanec);
                        break;

                    default:
                        MessageBox.Show("Unsupported entity type.");
                        break;
                }
            }
        }

        private void EditItem(object parameter)
        {
            if (parameter == null) return;

            var window = new EditWindow(parameter, _serviceProvider);
            if (window.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                Adresa.Clear();
                foreach (var adresaItem in await _adresaRepository.GetAllAddresses())
                    Adresa.Add(adresaItem);

                BlobTable.Clear();
                foreach (var blobItem in await _blobTableRepository.GetAllBlobTables())
                    BlobTable.Add(blobItem);

                Diagnosis.Clear();
                foreach (var diagItem in await _diagnozaRepository.GetAllDiagnoses())
                    Diagnosis.Add(diagItem);

                Hotovost.Clear();
                foreach (var hotItem in await _hotovostRepository.GetAllHotovost())
                    Hotovost.Add(hotItem);

                Karta.Clear();
                foreach (var kartaItem in await _kartaRepository.GetAllKarta())
                    Karta.Add(kartaItem);

                LekDiagnoza.Clear();
                foreach (var lekDiagItem in await _lekDiagnozaRepository.GetAllLekDiagnoza())
                    LekDiagnoza.Add(lekDiagItem);

                Lek.Clear();
                foreach (var lekItem in await _lekRepository.GetAllLeks())
                    Lek.Add(lekItem);

                Log.Clear();
                foreach (var logItem in await _logrepository.GetAllLogs())
                    Log.Add(logItem);

                NavstevaDiagnoza.Clear();
                foreach (var navDiagItem in await _navstevaDiagnozaRepository.GetAllNavstevaDiagnozas())
                    NavstevaDiagnoza.Add(navDiagItem);

                Navsteva.Clear();
                foreach (var navItem in await _navstevaRepository.GetAllNavstevas())
                    Navsteva.Add(navItem);

                Operace.Clear();
                foreach (var operItem in await _operaceRepository.GetAllOperaces())
                    Operace.Add(operItem);

                OperaceZamestnanec.Clear();
                foreach (var opZamItem in await _operaceZamestnanecRepository.GetAllOperaceZamestnanecs())
                    OperaceZamestnanec.Add(opZamItem);

                Ordinace.Clear();
                foreach (var ordItem in await _ordinaceRepository.GetAllOrdinaces())
                    Ordinace.Add(ordItem);

                OrdinaceZamestnanec.Clear();
                foreach (var ordZamItem in await _ordinaceZamestnanecRepository.GetAllOrdinaceZamestnanecs())
                    OrdinaceZamestnanec.Add(ordZamItem);

                Pacient.Clear();
                foreach (var pacItem in await _pacientRepository.GetAllPacients())
                    Pacient.Add(pacItem);

                Platba.Clear();
                foreach (var platbaItem in await _platbaRepository.GetAllPlatbas())
                    Platba.Add(platbaItem);

                Pozice.Clear();
                foreach (var pozItem in await _poziceRepository.GetAllPozices())
                    Pozice.Add(pozItem);

                Pripona.Clear();
                foreach (var priponaItem in await _priponaRepository.GetAllPriponas())
                    Pripona.Add(priponaItem);

                Status.Clear();
                foreach (var statItem in await _statusRepository.GetAllStatuses())
                    Status.Add(statItem);

                TypLek.Clear();
                foreach (var tLekItem in await _typLekRepository.GetAllTypLekes())
                    TypLek.Add(tLekItem);

                UzivatelData.Clear();
                foreach (var uzivItem in await _uzivatelDataRepository.GetAllUzivatelDatas())
                    UzivatelData.Add(uzivItem);

                ZamestnanecNavsteva.Clear();
                foreach (var zamNavItem in await _zamestnanecNavstevaRepository.GetAllZamestnanecNavstevas())
                    ZamestnanecNavsteva.Add(zamNavItem);

                Zamestnanec.Clear();
                foreach (var zamItem in await _zamestnanecRepository.GetAllZamestnanci())
                    Zamestnanec.Add(zamItem);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error or display a message)
            }
        }

        private async void ExecuteDelete(object item)
        {
            if (item == null) return;

            var result = MessageBox.Show("Ви впевнені, що хочете видалити цей запис?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                switch (item)
                {
                    case ADRESA adresa:
                        await _adresaRepository.DeleteAdresa(adresa.IdAdresa);
                        Adresa.Remove(adresa);
                        break;

                    case ZAMESTNANEC zamestnanec:
                        await _zamestnanecRepository.DeleteZamestnanec(zamestnanec.IdZamestnanec);
                        Zamestnanec.Remove(zamestnanec);
                        break;

                    case DIAGNOZA diagnoza:
                        await _diagnozaRepository.DeleteDiagnoza(diagnoza.IdDiagnoza);
                        Diagnosis.Remove(diagnoza);
                        break;

                    case ORDINACE ordinace:
                        await _ordinaceRepository.DeleteOrdinace(ordinace.IdOrdinace);
                        Ordinace.Remove(ordinace);
                        break;

                    case UZIVATEL_DATA uzivatelData:
                        await _uzivatelDataRepository.DeleteUzivatelData(uzivatelData.Id);
                        UzivatelData.Remove(uzivatelData);
                        break;

                    case PLATBA platba:
                        await _platbaRepository.DeletePlatba(platba.IdPlatba);
                        Platba.Remove(platba);
                        break;

                    case LOG log:
                        await _logrepository.DeleteLog(log.IdLog);
                        Log.Remove(log);
                        break;

                    case PACIENT pacient:
                        await _pacientRepository.DeletePacient(pacient.IdPacient);
                        Pacient.Remove(pacient);
                        break;

                    case POZICE pozice:
                        await _poziceRepository.DeletePozice(pozice.IdPozice);
                        Pozice.Remove(pozice);
                        break;

                    case PRIPONA pripona:
                        await _priponaRepository.DeletePripona(pripona.IdPripona);
                        Pripona.Remove(pripona);
                        break;

                    case LEK lek:
                        await _lekRepository.DeleteLek(lek.IdLek);
                        Lek.Remove(lek);
                        break;

                    case OPERACE operace:
                        await _operaceRepository.DeleteOperace(operace.IdOperace);
                        Operace.Remove(operace);
                        break;

                    case NAVSTEVA navsteva:
                        await _navstevaRepository.DeleteNavsteva(navsteva.IdNavsteva);
                        Navsteva.Remove(navsteva);
                        break;

                    case OPERACE_ZAMESTNANEC operaceZamestnanec:
                        await _operaceZamestnanecRepository.DeleteOperaceZamestnanec(operaceZamestnanec.OperaceId,
                            operaceZamestnanec.ZamestnanecId);
                        OperaceZamestnanec.Remove(operaceZamestnanec);
                        break;

                    case BLOB_TABLE blobTable:
                        await _blobTableRepository.DeleteBlob(blobTable.IdBlob);
                        BlobTable.Remove(blobTable);
                        break;

                    case ORDINACE_ZAMESTNANEC ordinaceZamestnanec:
                        await _ordinaceZamestnanecRepository.DeleteOrdinaceZamestnanec(ordinaceZamestnanec.OrdinaceId,
                            ordinaceZamestnanec.ZamestnanecId);
                        OrdinaceZamestnanec.Remove(ordinaceZamestnanec);
                        break;

                    case ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva:
                        await _zamestnanecNavstevaRepository.DeleteZamestnanecNavsteva(
                            zamestnanecNavsteva.ZamestnanecId, zamestnanecNavsteva.NavstevaId);
                        ZamestnanecNavsteva.Remove(zamestnanecNavsteva);
                        break;

                    case STATUS status:
                        await _statusRepository.DeleteStatus(status.IdStatus);
                        Status.Remove(status);
                        break;

                    case TYP_LEK typLek:
                        await _typLekRepository.DeleteTypLek(typLek.IdTypLek);
                        TypLek.Remove(typLek);
                        break;

                    case NAVSTEVA_DIAGNOZA navstevaDiagnoza:
                        await _navstevaDiagnozaRepository.DeleteNavstevaDiagnoza(navstevaDiagnoza.NavstevaId,
                            navstevaDiagnoza.DiagnozaId);
                        NavstevaDiagnoza.Remove(navstevaDiagnoza);
                        break;

                    default:
                        MessageBox.Show("Unsupported entity type.");
                        break;
                }

                MessageBox.Show("Запис успішно видалено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting item: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Фільтри
        private bool AdresaFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(AdresaSearchText))
                return true;

            var adresa = item as ADRESA;
            return adresa != null && (
                   (adresa.Stat != null && adresa.Stat.IndexOf(AdresaSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (adresa.Mesto != null && adresa.Mesto.IndexOf(AdresaSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (adresa.Ulice != null && adresa.Ulice.IndexOf(AdresaSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (adresa.CisloPopisne != null && adresa.CisloPopisne.ToString().IndexOf(AdresaSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool ZamestnanecFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(ZamestnanecSearchText))
                return true;

            var zamestnanec = item as ZAMESTNANEC;
            return zamestnanec != null && (
                   (zamestnanec.Jmeno != null && zamestnanec.Jmeno.IndexOf(ZamestnanecSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (zamestnanec.Prijmeni != null && zamestnanec.Prijmeni.IndexOf(ZamestnanecSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (zamestnanec.Telefon != null && zamestnanec.Telefon.ToString().IndexOf(ZamestnanecSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool DiagnózaFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(DiagnózaSearchText))
                return true;

            var diagnoza = item as DIAGNOZA;
            return diagnoza != null && (
                   (diagnoza.Nazev != null && diagnoza.Nazev.IndexOf(DiagnózaSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (diagnoza.Popis != null && diagnoza.Popis.IndexOf(DiagnózaSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool OrdinaceFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(OrdinaceSearchText))
                return true;

            var ordinace = item as ORDINACE;
            return ordinace != null && (
                   (ordinace.Nazev != null && ordinace.Nazev.IndexOf(OrdinaceSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool UzivatelskaDataFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(UzivatelskaDataSearchText))
                return true;

            var uzivatelskaData = item as UZIVATEL_DATA;
            return uzivatelskaData != null && (
                   (uzivatelskaData.Email != null && uzivatelskaData.Email.IndexOf(UzivatelskaDataSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (uzivatelskaData.Heslo != null && uzivatelskaData.Heslo.IndexOf(UzivatelskaDataSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool PlatbaFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(PlatbaSearchText))
                return true;

            var platba = item as PLATBA;
            return platba != null && (
                   platba.Castka.ToString().IndexOf(PlatbaSearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   platba.TypPlatby.IndexOf(PlatbaSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                   );
        }

        private bool PacientFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(PacientSearchText))
                return true;

            var pacient = item as PACIENT;
            return pacient != null && (
                   (pacient.Jmeno != null && pacient.Jmeno.IndexOf(PacientSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (pacient.Prijmeni != null && pacient.Prijmeni.IndexOf(PacientSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (pacient.RodneCislo != null && pacient.RodneCislo.ToString().IndexOf(PacientSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool PoziceFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(PoziceSearchText))
                return true;

            var pozice = item as POZICE;
            return pozice != null && (
                   (pozice.Nazev != null && pozice.Nazev.IndexOf(PoziceSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }
        private bool PriponaFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(PriponaSearchText))
                return true;

            var pripona = item as PRIPONA;
            return pripona != null && (
                pripona.Typ?.IndexOf(PriponaSearchText, StringComparison.OrdinalIgnoreCase) >= 0
            );
        }

        private bool StatusFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(StatusSearchText))
                return true;

            var status = item as STATUS;
            return status != null && (
                   (status.Nazev != null && status.Nazev.IndexOf(StatusSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool TypLekFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(TypLekSearchText))
                return true;

            var typLek = item as TYP_LEK;
            return typLek != null && (
                   (typLek.Nazev != null && typLek.Nazev.IndexOf(TypLekSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool LekFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(LekSearchText))
                return true;

            var lek = item as LEK;
            return lek != null && (
                   (lek.Nazev != null && lek.Nazev.IndexOf(LekSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool OperaceFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(OperaceSearchText))
                return true;

            var operace = item as OPERACE;
            return operace != null && (
                   (operace.Nazev != null && operace.Nazev.IndexOf(OperaceSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (operace.DiagnozaId.ToString().IndexOf(OperaceSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool NavstevaFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(NavstevaSearchText))
                return true;

            var navsteva = item as NAVSTEVA;
            return navsteva != null && (
                   (navsteva.MistnostId != null && navsteva.MistnostId.ToString().IndexOf(NavstevaSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool OperaceZamestnanecFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(OperaceZamestnanecSearchText))
                return true;

            var operaceZamestnanec = item as OPERACE_ZAMESTNANEC;
            return operaceZamestnanec != null && (
                   operaceZamestnanec.OperaceId.ToString().IndexOf(OperaceZamestnanecSearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   operaceZamestnanec.ZamestnanecId.ToString().IndexOf(OperaceZamestnanecSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                   );
        }

        private bool BlobTableFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(BlobTableSearchText))
                return true;

            var blob = item as BLOB_TABLE;
            return blob != null && (
                   (blob.NazevSouboru != null && blob.NazevSouboru.IndexOf(BlobTableSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (blob.TypSouboru != null && blob.TypSouboru.IndexOf(BlobTableSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                   );
        }

        private bool OrdinaceZamestnanecFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(OrdinaceZamestnanecSearchText))
                return true;

            var ordinaceZamestnanec = item as ORDINACE_ZAMESTNANEC;
            return ordinaceZamestnanec != null && (
                   ordinaceZamestnanec.OrdinaceId.ToString().IndexOf(OrdinaceZamestnanecSearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   ordinaceZamestnanec.ZamestnanecId.ToString().IndexOf(OrdinaceZamestnanecSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                   );
        }

        private bool ZamestnanecNavstevaFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(ZamestnanecNavstevaSearchText))
                return true;

            var zamestnanecNavsteva = item as ZAMESTNANEC_NAVSTEVA;
            return zamestnanecNavsteva != null && (
                   zamestnanecNavsteva.ZamestnanecId.ToString().IndexOf(ZamestnanecNavstevaSearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   zamestnanecNavsteva.NavstevaId.ToString().IndexOf(ZamestnanecNavstevaSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                   );
        }

        private bool NavstevaDiagnozaFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(NavstevaDiagnozaSearchText))
                return true;

            var navstevaDiagnoza = item as NAVSTEVA_DIAGNOZA;
            return navstevaDiagnoza != null && (
                   navstevaDiagnoza.NavstevaId.ToString().IndexOf(NavstevaDiagnozaSearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   navstevaDiagnoza.DiagnozaId.ToString().IndexOf(NavstevaDiagnozaSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                   );
        }
    }
}
