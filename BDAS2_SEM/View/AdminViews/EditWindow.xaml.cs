// EditWindow.xaml.cs

using System;
using System.Windows;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.View.AdminViews
{
    public partial class EditWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly object _item;

        public EditWindow(object item, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _item = item;
            this.DataContext = _item;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_item)
                {
                    case ADRESA adresa:
                        var adresaRepo = _serviceProvider.GetService<IAdresaRepository>();
                        if (adresaRepo != null)
                        {
                            await adresaRepo.UpdateAdresa(adresa.IdAdresa, adresa);
                        }
                        break;

                    case ZAMESTNANEC zamestnanec:
                        var zamRepo = _serviceProvider.GetService<IZamestnanecRepository>();
                        if (zamRepo != null)
                        {
                            await zamRepo.UpdateZamestnanec(zamestnanec);
                        }
                        break;

                    // Add cases for other models
                    case OPERACE operace:
                        var operaceRepo = _serviceProvider.GetService<IOperaceRepository>();
                        if (operaceRepo != null)
                        {
                            await operaceRepo.UpdateOperace(operace);
                        }
                        break;

                    case HOTOVOST hotovost:
                        var hotovostRepo = _serviceProvider.GetService<IHotovostRepository>();
                        if (hotovostRepo != null)
                        {
                            await hotovostRepo.UpdateHotovost(hotovost);
                        }
                        break;

                    // Continue for all other models
                    default:
                        MessageBox.Show("Unsupported entity type.");
                        return;
                }

                MessageBox.Show("Item updated successfully.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}");
            }
        }
    }
}