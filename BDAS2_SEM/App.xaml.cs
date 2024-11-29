using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Repository;
using BDAS2_SEM.View;
using BDAS2_SEM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.Services;
using BDAS2_SEM.View.AdminViews;
using BDAS2_SEM.View.PatientViews;
using BDAS2_SEM.View.DoctorViews;

namespace BDAS2_SEM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        private void ConfigureServices(IServiceCollection services)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleDbConnection"].ConnectionString;

            // Register repositories
            services.AddSingleton<IAdresaRepository, AdresaRepository>(provider =>
                new AdresaRepository(connectionString));
            services.AddSingleton<IDiagnozaRepository, DiagnozaRepository>(provider =>
                new DiagnozaRepository(connectionString));
            services.AddSingleton<IHotovostRepository, HotovostRepository>(provider =>
                new HotovostRepository(connectionString));
            services.AddSingleton<IKartaRepository, KartaRepository>(provider =>
                new KartaRepository(connectionString));
            services.AddSingleton<ILekDiagnozaRepository, LekDiagnozaRepository>(provider =>
                new LekDiagnozaRepository(connectionString));
            services.AddSingleton<ILekRepository, LekRepository>(provider =>
                new LekRepository(connectionString));
            services.AddSingleton<INavstevaDiagnozaRepository, NavstevaDiagnozaRepository>(provider =>
                new NavstevaDiagnozaRepository(connectionString));
            services.AddSingleton<INavstevaRepository, NavstevaRepository>(provider =>
                new NavstevaRepository(connectionString));
            services.AddSingleton<IOperaceRepository, OperaceRepository>(provider =>
                new OperaceRepository(connectionString));
            services.AddSingleton<IOperaceZamestnanecRepository, OperaceZamestnanecRepository>(provider =>
                new OperaceZamestnanecRepository(connectionString));
            services.AddSingleton<IOrdinaceRepository, OrdinaceRepository>(provider =>
                new OrdinaceRepository(connectionString));
            services.AddSingleton<IOrdinaceZamestnanecRepository, OrdinaceZamestnanecRepository>(provider =>
                new OrdinaceZamestnanecRepository(connectionString));
            services.AddSingleton<IPacientRepository, PacientRepository>(provider =>
                new PacientRepository(connectionString));
            services.AddSingleton<IPlatbaRepository, PlatbaRepository>(provider =>
                new PlatbaRepository(connectionString));
            services.AddSingleton<IPoziceRepository, PoziceRepository>(provider =>
                new PoziceRepository(connectionString));
            services.AddSingleton<IUzivatelDataRepository, UzivatelDataRepository>(provider =>
                new UzivatelDataRepository(connectionString));
            services.AddSingleton<IZamestnanecNavstevaRepository, ZamestnanecNavstevaRepository>(provider =>
                new ZamestnanecNavstevaRepository(connectionString));
            services.AddSingleton<IZamestnanecRepository, ZamestnanecRepository>(provider =>
                new ZamestnanecRepository(connectionString));

            // Register services
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<IPatientContextService, PatientContextService>();
            // Add other services if needed

            // Register ViewModels
            services.AddTransient<AuthVM>();
            services.AddTransient<DoctorsVM>();
            services.AddTransient<MainDoctorVM>();
            services.AddTransient<PatientsVM>();
            services.AddTransient<AdminVM>();
            services.AddTransient<AppointmentsVM>();
            services.AddTransient<PAppointmentsVM>();
            services.AddTransient<DoctorsListVM>();
            services.AddTransient<NewUsersVM>();
            services.AddTransient<AddAddressVM>();
            services.AddTransient<AddPositionVM>();
            services.AddTransient<NewEmployeeVM>();
            services.AddTransient<NewPatientVM>();
            services.AddTransient<TabItemVM>();
            services.AddTransient<AssignAppointmentVM>();
            // Register other ViewModels if needed

            // Register Views
            services.AddTransient<AuthWindow>();
            services.AddTransient<DoctorsWindow>();
            services.AddTransient<MainDoctorView>();
            services.AddTransient<PatientsWindow>();
            services.AddTransient<AdminWindow>();
            services.AddTransient<NewUsersView>();
            services.AddSingleton<NewEmployeeWindow>();
            services.AddSingleton<AddAddressWindow>();
            services.AddSingleton<NewPatientWindow>();
            services.AddTransient<AppointmentsView>();
            services.AddTransient<PAppointmentsView>();
            services.AddTransient<DoctorsListView>();
            services.AddTransient<AssignAppointmentView>();
            // Register other Views if needed

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var authWindow = ServiceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();
        }
    }

}
