using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using System.IO;

namespace BDAS2_SEM.ViewModel
{
    public class DSettingsVM : INotifyPropertyChanged
    {
        private ZAMESTNANEC _doctor;
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IBlobTableRepository _blobRepository;
        private readonly IWindowService _windowService;
        private readonly Action<byte[]> _updateAvatarAction;

        public DSettingsVM(ZAMESTNANEC doctor,
                           IZamestnanecRepository zamestnanecRepository,
                           IBlobTableRepository blobRepository,
                           IWindowService windowService,
                           Action<byte[]> updateAvatarAction)
        {
            _doctor = doctor;
            _zamestnanecRepository = zamestnanecRepository;
            _blobRepository = blobRepository;
            _windowService = windowService;
            _updateAvatarAction = updateAvatarAction;

            AddOrChangeImageCommand = new RelayCommand(AddOrChangeImage);

            LoadDoctorData();
        }

        // Свойства для биндинга в интерфейсе

        public ZAMESTNANEC Doctor
        {
            get => _doctor;
            set
            {
                _doctor = value;
                OnPropertyChanged();
                LoadDoctorData();
            }
        }

        // Команды
        public ICommand AddOrChangeImageCommand { get; }

        // Методы

        private async void LoadDoctorData()
        {
            if (_doctor != null)
            {
                var doctorData = await _zamestnanecRepository.GetZamestnanecById(_doctor.IdZamestnanec);
                if (doctorData != null)
                {
                    Doctor = doctorData;
                }
            }
        }

        private async void AddOrChangeImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Читаем выбранный файл
                byte[] imageData = File.ReadAllBytes(openFileDialog.FileName);

                // Сохраняем изображение в базе данных
                var existingBlob = await _blobRepository.GetBlobByZamestnanecId(_doctor.IdZamestnanec);

                BLOB_TABLE blob = new BLOB_TABLE
                {
                    NazevSouboru = Path.GetFileName(openFileDialog.FileName),
                    TypSouboru = "Image",
                    PriponaSouboru = Path.GetExtension(openFileDialog.FileName),
                    Obsah = imageData,
                    DatumNahrani = DateTime.Now,
                    DatumModifikace = DateTime.Now,
                    OperaceProvedl = $"{_doctor.Jmeno} {_doctor.Prijmeni}",
                    PopisOperace = "Добавление аватарки",
                    ZamestnanecId = _doctor.IdZamestnanec
                };

                if (existingBlob != null)
                {
                    // Обновляем существующую запись
                    blob.IdBlob = existingBlob.IdBlob;
                    await _blobRepository.UpdateBlobContent(blob);
                }
                else
                {
                    // Добавляем новую запись
                    await _blobRepository.AddBlobContent(blob);
                }

                // Обновляем аватарку в главном меню
                _updateAvatarAction(imageData);
            }
        }

        // Реализация INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}