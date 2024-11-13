using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using BDAS2_SEM.View;

namespace BDAS2_SEM.ViewModel
{
    public class AddAddressVM : INotifyPropertyChanged
    {
        private readonly IAdresaRepository _adresaRepository;
        private readonly Action<ADRESA> _onAddressAdded;
        private readonly IWindowService _windowService;

        public string Stat { get; set; }
        public string Mesto { get; set; }
        public int PSC { get; set; }
        public string Ulice { get; set; }
        public int CisloPopisne { get; set; }

        public ICommand SaveCommand { get; }

        public AddAddressVM(Action<ADRESA> onAddressAdded, IWindowService windowService, IServiceProvider serviceProvider)
        {
            _onAddressAdded = onAddressAdded;
            _windowService = windowService;
            _adresaRepository = serviceProvider.GetRequiredService<IAdresaRepository>();

            SaveCommand = new RelayCommand(Save);
        }

        private async void Save(object parameter)
        {
            var newAddress = new ADRESA
            {
                Stat = this.Stat,
                Mesto = this.Mesto,
                PSC = this.PSC,
                Ulice = this.Ulice,
                CisloPopisne = this.CisloPopisne
            };

            int id = await _adresaRepository.AddNewAdresa(newAddress);
            newAddress.IdAdresa = id;

            _onAddressAdded?.Invoke(newAddress);

            _windowService.CloseWindow(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is AddAddressWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}