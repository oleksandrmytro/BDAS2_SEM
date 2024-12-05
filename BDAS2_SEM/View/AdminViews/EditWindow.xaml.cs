// EditWindow.xaml.cs

using System;
using System.Reflection;
using System.Windows;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace BDAS2_SEM.View.AdminViews
{
    public partial class EditWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly object _originalItem;
        private readonly object _itemCopy;
        private readonly bool _isNewItem;

        public EditWindow(object item, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _originalItem = item;
            _itemCopy = CreateCopy(item);
            this.DataContext = _itemCopy;

            _isNewItem = IsNewItem(_itemCopy);
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CopyProperties(_itemCopy, _originalItem);

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
                        var hotovostRepo = _serviceProvider.GetService<IHotovostRepository>();
                        if (hotovostRepo != null)
                        {
                            if (_isNewItem)
                                await hotovostRepo.AddHotovost(hotovost);
                            else
                                await hotovostRepo.UpdateHotovost(hotovost);
                        }
                        break;

                    case KARTA karta:
                        var kartaRepo = _serviceProvider.GetService<IKartaRepository>();
                        if (kartaRepo != null)
                        {
                            if (_isNewItem)
                                await kartaRepo.AddKarta(karta);
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

                    default:
                        MessageBox.Show("Unsupported entity type.");
                        return;
                }

                MessageBox.Show(_isNewItem ? "Елемент успішно додано." : "Елемент успішно оновлено.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item: {ex.Message}");
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
    }
}