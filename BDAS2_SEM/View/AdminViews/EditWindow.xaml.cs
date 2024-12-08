using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace BDAS2_SEM.View.AdminViews
{
    public partial class EditWindow : Window
    {
        private readonly EditWindowViewModel _viewModel;

        public EditWindow(object item, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Створюємо екземпляр ViewModel та встановлюємо його як DataContext
            _viewModel = new EditWindowViewModel(item, serviceProvider);
            this.DataContext = _viewModel;

            // Завантажуємо дані для комбобоксів
            _viewModel.LoadComboBoxDataAsync();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveChangesAsync();
        }
    }

    public class EditWindowViewModel : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly object _originalItem;
        private readonly bool _isNewItem;

        public object Item { get; set; }
        public ObservableCollection<ADRESA> AdresaList { get; set; }
        public ObservableCollection<POZICE> PoziceList { get; set; }
        public ObservableCollection<TYP_LEK> TypLeks { get; set; }
        public ObservableCollection<ROLE> AvailableRoles { get; set; }
        public ObservableCollection<ORDINACE> OrdinaceList { get; set; }

        private decimal? _castkaIn;
        public decimal? CastkaIn
        {
            get => _castkaIn;
            set
            {
                if (_castkaIn != value)
                {
                    _castkaIn = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _datumDate;
        public DateTime? DatumDate
        {
            get => _datumDate;
            set
            {
                if (_datumDate != value)
                {
                    _datumDate = value;
                    OnPropertyChanged();
                    UpdateFullDatum();
                }
            }
        }

        private string _datumTime;
        public string DatumTime
        {
            get => _datumTime;
            set
            {
                if (_datumTime != value)
                {
                    _datumTime = value;
                    OnPropertyChanged();
                    UpdateFullDatum();
                }
            }
        }

        // Властивості для управління типом оплати
        private string _selectedPaymentMethod;
        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                if (_selectedPaymentMethod != value)
                {
                    _selectedPaymentMethod = value;
                    OnPropertyChanged();
                    UpdatePaymentMethodFlags();
                }
            }
        }

        private bool _isKartaMethod;
        public bool IsKartaMethod
        {
            get => _isKartaMethod;
            set
            {
                if (_isKartaMethod != value)
                {
                    _isKartaMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isHotovostMethod;
        public bool IsHotovostMethod
        {
            get => _isHotovostMethod;
            set
            {
                if (_isHotovostMethod != value)
                {
                    _isHotovostMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        // Властивості специфічні для картки
        private decimal? _cardNumber;
        public decimal? CardNumber
        {
            get => _cardNumber;
            set
            {
                if (_cardNumber != value)
                {
                    _cardNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        // Властивості специфічні для готівки
        private decimal? _cashGiven;
        public decimal? CashGiven
        {
            get => _cashGiven;
            set
            {
                if (_cashGiven != value)
                {
                    _cashGiven = value;
                    OnPropertyChanged();
                    UpdateChange();
                }
            }
        }

        private decimal? _change;
        public decimal? Change
        {
            get => _change;
            set
            {
                if (_change != value)
                {
                    _change = value;
                    OnPropertyChanged();
                }
            }
        }


        public EditWindowViewModel(object item, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _originalItem = item;
            Item = CreateCopy(item);

            // Визначаємо, чи є це новий елемент
            _isNewItem = IsNewItem(Item);

            AdresaList = new ObservableCollection<ADRESA>();
            PoziceList = new ObservableCollection<POZICE>();
            TypLeks = new ObservableCollection<TYP_LEK>();
            AvailableRoles = new ObservableCollection<ROLE>();
            OrdinaceList = new ObservableCollection<ORDINACE>();

            // Якщо Item це PLATBA, ініціалізуємо прапорці та підписуємося на PropertyChanged
            if (Item is PLATBA platbaItem)
            {
                platbaItem.PropertyChanged += PlatbaItem_PropertyChanged;

                // Ініціалізуємо властивості ViewModel на основі моделі PLATBA
                DatumDate = platbaItem.Datum.Date;
                DatumTime = platbaItem.Datum.ToString("HH:mm:ss");
                SelectedPaymentMethod = platbaItem.TypPlatby;

                // Ініціалізуємо специфічні поля залежно від типу оплати
                if (platbaItem.TypPlatby.Equals("karta", StringComparison.OrdinalIgnoreCase))
                {
                    CardNumber = platbaItem.CisloKarty;
                }
                else if (platbaItem.TypPlatby.Equals("hotovost", StringComparison.OrdinalIgnoreCase))
                {
                    CashGiven = platbaItem.Prijato;
                    Change = platbaItem.Vraceno;
                }

                UpdatePaymentMethodFlags(platbaItem.TypPlatby);
            }

            // Якщо Item це KARTA, ініціалізуємо відповідні властивості
            if (Item is KARTA kartaItem)
            {
                CardNumber = kartaItem.CisloKarty;

                if (!_isNewItem)
                {
                    var platbaRepo = _serviceProvider.GetService<IPlatbaRepository>();
                    if (platbaRepo != null)
                    {
                        var platba = platbaRepo.GetPlatbaById(kartaItem.IdPlatba).Result;
                        if (platba != null)
                        {
                            CastkaIn = platba.Castka;
                        }
                    }
                }
            }
        }

        private void PlatbaItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PLATBA.TypPlatby))
            {
                if (sender is PLATBA platbaItem)
                {
                    UpdatePaymentMethodFlags(platbaItem.TypPlatby);
                }
            }
        }

        private void UpdatePaymentMethodFlags()
        {
            // Оновлюємо прапорці на основі вибраного типу оплати
            IsKartaMethod = SelectedPaymentMethod.Equals("karta", StringComparison.OrdinalIgnoreCase);
            IsHotovostMethod = SelectedPaymentMethod.Equals("hotovost", StringComparison.OrdinalIgnoreCase);
        }

        private void UpdatePaymentMethodFlags(string typPlatby)
        {
            IsKartaMethod = typPlatby.Equals("karta", StringComparison.OrdinalIgnoreCase);
            IsHotovostMethod = typPlatby.Equals("hotovost", StringComparison.OrdinalIgnoreCase);
        }

        private void UpdateChange()
        {
            if (Item is PLATBA platbaItem)
            {
                platbaItem.Vraceno = CashGiven - platbaItem.Castka;
                Change = platbaItem.Vraceno;
            }
        }

        private void UpdateFullDatum()
        {
            // Якщо Item - PLATBA, то складаємо дату і час назад в одне поле
            if (Item is PLATBA platbaItem && DatumDate.HasValue && TimeSpan.TryParse(DatumTime, out var time))
            {
                platbaItem.Datum = DatumDate.Value.Date + time;
            }
        }

        public async void LoadComboBoxDataAsync()
        {
            try
            {
                var adresaRepo = _serviceProvider.GetService<IAdresaRepository>();
                var poziceRepo = _serviceProvider.GetService<IPoziceRepository>();
                var typLekRepo = _serviceProvider.GetService<ITypLekRepository>();
                var roleRepo = _serviceProvider.GetService<IRoleRepository>();
                var ordinRepo = _serviceProvider.GetService<IOrdinaceRepository>();

                if (adresaRepo != null)
                {
                    var adresaItems = await adresaRepo.GetAllAddresses();
                    foreach (var adresa in adresaItems)
                        AdresaList.Add(adresa);
                }

                if (poziceRepo != null)
                {
                    var poziceItems = await poziceRepo.GetAllPozices();
                    foreach (var pozice in poziceItems)
                        PoziceList.Add(pozice);
                }

                if (typLekRepo != null)
                {
                    var typLekItems = await typLekRepo.GetAllTypLekes();
                    foreach (var typLek in typLekItems)
                        TypLeks.Add(typLek);
                }

                if (roleRepo != null)
                {
                    var roleItems = await roleRepo.GetAllRoles();
                    foreach (var role in roleItems)
                        AvailableRoles.Add(role);
                }

                if (ordinRepo != null)
                {
                    var ordinaces = await ordinRepo.GetAllOrdinaces();
                    foreach (var ordin in ordinaces)
                    {
                        OrdinaceList.Add(ordin);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ComboBox data: {ex.Message}");
            }
        }

        public async void SaveChangesAsync()
        {
            try
            {
                CopyProperties(Item, _originalItem);

                // Якщо Item це PLATBA, встановлюємо додаткові поля
                if (_originalItem is PLATBA platbaItem)
                {
                    // Переконайтеся, що всі специфічні поля встановлені
                    platbaItem.TypPlatby = SelectedPaymentMethod;

                    if (IsKartaMethod)
                    {
                        platbaItem.CisloKarty = CardNumber;
                        platbaItem.Prijato = null;
                        platbaItem.Vraceno = null;
                    }
                    else if (IsHotovostMethod)
                    {
                        platbaItem.Prijato = CashGiven;
                        platbaItem.Vraceno = Change;
                        platbaItem.CisloKarty = null;
                    }

                    // Оновлюємо дату та час
                    UpdateFullDatum();
                }

                switch (_originalItem)
                {
                    case ADRESA adresa:
                        var adresaRepo = _serviceProvider.GetService<IAdresaRepository>();
                        if (adresaRepo != null)
                        {
                            if (_isNewItem)
                                await adresaRepo.AddAdresa(adresa);
                            else
                                await adresaRepo.UpdateAdresa(adresa.IdAdresa, adresa);
                        }
                        break;

                    case ZAMESTNANEC zamestnanec:
                        var zamRepo = _serviceProvider.GetService<IZamestnanecRepository>();
                        if (zamRepo != null)
                        {
                            if (_isNewItem)
                                await zamRepo.AddZamestnanec(zamestnanec);
                            else
                                await zamRepo.UpdateZamestnanec(zamestnanec);
                        }
                        break;

                    case BLOB_TABLE blobTable:
                        var blobRepo = _serviceProvider.GetService<IBlobTableRepository>();
                        if (blobRepo != null)
                        {
                            if (_isNewItem)
                                await blobRepo.AddBlob(blobTable);
                            else
                                await blobRepo.UpdateBlob(blobTable);
                        }

                        break;

                    case DIAGNOZA diagnoza:
                        var diagnozaRepo = _serviceProvider.GetService<IDiagnozaRepository>();
                        if (diagnozaRepo != null)
                        {
                            if (_isNewItem)
                                await diagnozaRepo.AddDiagnoza(diagnoza);
                            else
                                await diagnozaRepo.UpdateDiagnoza(diagnoza);
                        }

                        break;

                    case HOTOVOST hotovost:
                        var platbaHotRepo = _serviceProvider.GetService<IPlatbaRepository>();
                        var hotovostRepo = _serviceProvider.GetService<IHotovostRepository>();
                        if (platbaHotRepo != null && hotovostRepo != null)
                        {
                            if (_isNewItem)
                            {
                                // Створюємо новий об'єкт PLATBA, якщо його ще немає
                                var newPlatba = new PLATBA
                                {
                                    // Ініціалізуйте необхідні властивості PLATBA
                                    Prijato = hotovost.Prijato,
                                    Vraceno = hotovost.Vraceno,
                                    TypPlatby = "hotovost",
                                    Datum = DateTime.Now,
                                    CisloKarty = null,
                                    Castka = hotovost.Prijato - hotovost.Vraceno,
                                    NavstevaId = null,
                                    // Додайте інші властивості за потребою
                                };

                                // Вставляємо PLATBA і отримуємо його ID
                                await platbaHotRepo.AddPlatba(newPlatba);

                            }
                            else
                            {
                                // Якщо це не новий елемент, оновлюємо PLATBA та HOTOVOST
                                
                            }
                        }
                        else
                        {
                            MessageBox.Show("Помилка: Не вдалося отримати необхідні репозиторії.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        break;


                    case KARTA karta:
                        var platbaKarRepo = _serviceProvider.GetService<IPlatbaRepository>();
                        var kartaRepo = _serviceProvider.GetService<IKartaRepository>();
                        if (kartaRepo != null)
                        {
                            if (_isNewItem)
                            {
                                var newPlatba = new PLATBA
                                {
                                    // Ініціалізуйте необхідні властивості PLATBA
                                    Prijato = null,
                                    Vraceno = null,
                                    TypPlatby = "karta",
                                    Datum = DateTime.Now,
                                    CisloKarty = karta.CisloKarty,
                                    Castka = CastkaIn,
                                    NavstevaId = null,
                                    // Додайте інші властивості за потребою
                                };

                                await platbaKarRepo.AddPlatba(newPlatba);
                            }
                            else
                                await kartaRepo.UpdateKarta(karta);
                        }

                        break;

                    case LEK_DIAGNOZA lekDiagnoza:
                        var lekDiagnozaRepo = _serviceProvider.GetService<ILekDiagnozaRepository>();
                        if (lekDiagnozaRepo != null)
                        {
                            if (_isNewItem)
                                await lekDiagnozaRepo.AddLekDiagnoza(lekDiagnoza);
                            else
                                await lekDiagnozaRepo.UpdateLekDiagnoza(lekDiagnoza);
                        }

                        break;

                    case LEK lek:
                        var lekRepo = _serviceProvider.GetService<ILekRepository>();
                        if (lekRepo != null)
                        {
                            if (_isNewItem)
                                await lekRepo.AddLek(lek);
                            else
                                await lekRepo.UpdateLek(lek);
                        }

                        break;

                    case NAVSTEVA_DIAGNOZA navstevaDiagnoza:
                        var navstevaDiagnozaRepo = _serviceProvider.GetService<INavstevaDiagnozaRepository>();
                        if (navstevaDiagnozaRepo != null)
                        {
                            if (_isNewItem)
                                await navstevaDiagnozaRepo.AddNavstevaDiagnoza(navstevaDiagnoza);
                            else
                                await navstevaDiagnozaRepo.UpdateNavstevaDiagnoza(navstevaDiagnoza);
                        }

                        break;

                    case NAVSTEVA navsteva:
                        var navstevaRepo = _serviceProvider.GetService<INavstevaRepository>();
                        if (navstevaRepo != null)
                        {
                            if (_isNewItem)
                                await navstevaRepo.AddNavsteva(navsteva);
                            else
                                await navstevaRepo.UpdateNavsteva(navsteva);
                        }

                        break;

                    case OPERACE operace:
                        var operaceRepo = _serviceProvider.GetService<IOperaceRepository>();
                        if (operaceRepo != null)
                        {
                            if (_isNewItem)
                                await operaceRepo.AddOperace(operace);
                            else
                                await operaceRepo.UpdateOperace(operace);
                        }

                        break;

                    case OPERACE_ZAMESTNANEC operaceZamestnanec:
                        var operaceZamRepo = _serviceProvider.GetService<IOperaceZamestnanecRepository>();
                        if (operaceZamRepo != null)
                        {
                            if (_isNewItem)
                                await operaceZamRepo.AddOperaceZamestnanec(operaceZamestnanec);
                            else
                                await operaceZamRepo.UpdateOperaceZamestnanec(operaceZamestnanec);
                        }

                        break;

                    case ORDINACE ordinace:
                        var ordinaceRepo = _serviceProvider.GetService<IOrdinaceRepository>();
                        if (ordinaceRepo != null)
                        {
                            if (_isNewItem)
                                await ordinaceRepo.AddOrdinace(ordinace);
                            else
                                await ordinaceRepo.UpdateOrdinace(ordinace);
                        }

                        break;

                    case ORDINACE_ZAMESTNANEC ordinaceZamestnanec:
                        var ordinaceZamRepo = _serviceProvider.GetService<IOrdinaceZamestnanecRepository>();
                        if (ordinaceZamRepo != null)
                        {
                            if (_isNewItem)
                                await ordinaceZamRepo.AddOrdinaceZamestnanec(ordinaceZamestnanec);
                            else
                                await ordinaceZamRepo.UpdateOrdinaceZamestnanec(ordinaceZamestnanec);
                        }

                        break;

                    case PACIENT pacient:
                        var pacientRepo = _serviceProvider.GetService<IPacientRepository>();
                        if (pacientRepo != null)
                        {
                            if (_isNewItem)
                                await pacientRepo.AddPacient(pacient);
                            else
                                await pacientRepo.UpdatePacient(pacient);
                        }

                        break;

                    case PLATBA platba:
                        // Дата і час вже зібрані в UpdateFullDatum()
                        var platbaRepo = _serviceProvider.GetService<IPlatbaRepository>();
                        if (platbaRepo != null)
                        {
                            if (_isNewItem)
                                await platbaRepo.AddPlatba(platba);
                            else
                                await platbaRepo.UpdatePlatba(platba);
                        }

                        break;

                    case POZICE pozice:
                        var poziceRepo = _serviceProvider.GetService<IPoziceRepository>();
                        if (poziceRepo != null)
                        {
                            if (_isNewItem)
                                await poziceRepo.AddPozice(pozice);
                            else
                                await poziceRepo.UpdatePozice(pozice);
                        }

                        break;

                    case PRIPONA pripona:
                        var priponaRepo = _serviceProvider.GetService<IPriponaRepository>();
                        if (priponaRepo != null)
                        {
                            if (_isNewItem)
                                await priponaRepo.AddPripona(pripona);
                            else
                                await priponaRepo.UpdatePripona(pripona);
                        }

                        break;

                    case ROLE role:
                        var roleRepo = _serviceProvider.GetService<IRoleRepository>();
                        if (roleRepo != null)
                        {
                            if (_isNewItem)
                                await roleRepo.AddRole(role);
                            else
                                await roleRepo.UpdateRole(role);
                        }

                        break;

                    case STATUS status:
                        var statusRepo = _serviceProvider.GetService<IStatusRepository>();
                        if (statusRepo != null)
                        {
                            if (_isNewItem)
                                await statusRepo.AddStatus(status);
                            else
                                await statusRepo.UpdateStatus(status);
                        }

                        break;

                    case TYP_LEK typLek:
                        var typLekRepo = _serviceProvider.GetService<ITypLekRepository>();
                        if (typLekRepo != null)
                        {
                            if (_isNewItem)
                                await typLekRepo.AddTypLek(typLek);
                            else
                                await typLekRepo.UpdateTypLek(typLek);
                        }

                        break;

                    case UZIVATEL_DATA uzivatelData:
                        var uzivatelDataRepo = _serviceProvider.GetService<IUzivatelDataRepository>();
                        if (uzivatelDataRepo != null)
                        {
                            if (_isNewItem)
                                await uzivatelDataRepo.RegisterNewUserData(uzivatelData.Email, uzivatelData.Heslo);
                            else
                                await uzivatelDataRepo.UpdateUserData(uzivatelData);
                        }

                        break;

                    case MISTNOST mistnost:
                        var mistnostRepo = _serviceProvider.GetRequiredService<IMistnostRepository>();
                        if (mistnostRepo != null)
                        {
                            if (_isNewItem)
                                await mistnostRepo.AddMistnost(mistnost);
                            else
                                await mistnostRepo.UpdateMistnost(mistnost);
                        }

                        break;


                    case ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva:
                        var zamNavRepo = _serviceProvider.GetService<IZamestnanecNavstevaRepository>();
                        if (zamNavRepo != null)
                        {
                            if (_isNewItem)
                                await zamNavRepo.AddZamestnanecNavsteva(zamestnanecNavsteva);
                            else
                                await zamNavRepo.UpdateZamestnanecNavsteva(zamestnanecNavsteva);
                        }

                        break;

                    default:
                        MessageBox.Show("Unsupported entity type.");
                        return;
                }

                MessageBox.Show(_isNewItem ? "Елемент успішно додано." : "Елемент успішно оновлено.");
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}");
            }
        }

        private void CloseWindow()
        {
            // Закриття вікна після збереження
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }

        private bool IsNewItem(object item)
        {
            switch (item)
            {
                case ADRESA adresa:
                    return adresa.IdAdresa == 0;

                case BLOB_TABLE blobTable:
                    return blobTable.IdBlob == 0;

                case DIAGNOZA diagnoza:
                    return diagnoza.IdDiagnoza == 0;

                case HOTOVOST hotovost:
                    return hotovost.IdPlatba == 0;

                case KARTA karta:
                    return karta.IdPlatba == 0;

                case LEK_DIAGNOZA lekDiagnoza:
                    return lekDiagnoza.LekId == 0 && lekDiagnoza.DiagnozaId == 0;

                case LEK lek:
                    return lek.IdLek == 0;

                case NAVSTEVA_DIAGNOZA navstevaDiagnoza:
                    return navstevaDiagnoza.NavstevaId == 0 && navstevaDiagnoza.DiagnozaId == 0;

                case NAVSTEVA navsteva:
                    return navsteva.IdNavsteva == 0;

                case OPERACE operace:
                    return operace.IdOperace == 0;

                case OPERACE_ZAMESTNANEC operaceZamestnanec:
                    return operaceZamestnanec.OperaceId == 0 && operaceZamestnanec.ZamestnanecId == 0;

                case ORDINACE ordinace:
                    return ordinace.IdOrdinace == 0;

                case ORDINACE_ZAMESTNANEC ordinaceZamestnanec:
                    return ordinaceZamestnanec.OrdinaceId == 0 && ordinaceZamestnanec.ZamestnanecId == 0;

                case PACIENT pacient:
                    return pacient.IdPacient == 0;

                case PLATBA platba:
                    return platba.IdPlatba == 0;

                case POZICE pozice:
                    return pozice.IdPozice == 0;

                case PRIPONA pripona:
                    return pripona.IdPripona == 0;

                case MISTNOST mistnost:
                    return mistnost.IdMistnost == 0;

                case TYP_LEK typLek:
                    return typLek.IdTypLek == 0;

                case ROLE role:
                    return role.IdRole == 0;

                case STATUS status:
                    return status.IdStatus == 0;

                case UZIVATEL_DATA uzivatelData:
                    return uzivatelData.Id == 0;

                case ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva:
                    return zamestnanecNavsteva.ZamestnanecId == 0 && zamestnanecNavsteva.NavstevaId == 0;

                case ZAMESTNANEC zamestnanec:
                    return zamestnanec.IdZamestnanec == 0;

                default:
                    return false;
            }
        }

        private object CreateCopy(object source)
        {
            if (source == null)
                return null;

            var type = source.GetType();
            var copy = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var value = prop.GetValue(source);
                    prop.SetValue(copy, value);
                }
            }

            return copy;
        }

        private void CopyProperties(object source, object destination)
        {
            if (source == null || destination == null)
                return;

            var type = source.GetType();

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var value = prop.GetValue(source);
                    prop.SetValue(destination, value);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
