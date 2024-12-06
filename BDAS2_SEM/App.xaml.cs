using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Repository;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.Services;
using BDAS2_SEM.View.AdminViews;
using BDAS2_SEM.View.DoctorViews;
using BDAS2_SEM.View.PatientViews;
using BDAS2_SEM.View;
using BDAS2_SEM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Configuration;

namespace BDAS2_SEM
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private void ConfigureServices(IServiceCollection services)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OracleDbConnection"].ConnectionString;

            // Register repositories
            services.AddSingleton<IAdresaRepository, AdresaRepository>(provider =>
                new AdresaRepository(connectionString));
            services.AddSingleton<IBlobTableRepository, BlobTableRepository>(provider =>
                new BlobTableRepository(connectionString));
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
            services.AddSingleton<ILogRepository, LogRepository>(provider =>
                new LogRepository(connectionString));
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
            services.AddSingleton<IStatusRepository, StatusRepository>(provider =>
                new StatusRepository(connectionString));
            services.AddSingleton<ITypLekRepository, TypLekRepository>(provider =>
                new TypLekRepository(connectionString));
            services.AddSingleton<IUzivatelDataRepository, UzivatelDataRepository>(provider =>
                new UzivatelDataRepository(connectionString));
            services.AddSingleton<IZamestnanecNavstevaRepository, ZamestnanecNavstevaRepository>(provider =>
                new ZamestnanecNavstevaRepository(connectionString));
            services.AddSingleton<IZamestnanecRepository, ZamestnanecRepository>(provider =>
                new ZamestnanecRepository(connectionString));
            // IBlobTableRepository
            services.AddSingleton<IBlobTableRepository, BlobTableRepository>(provider =>
                new BlobTableRepository(connectionString));
            services.AddSingleton<ISystemCatalogRepository, SystemCatalogRepository>(provider =>
                new SystemCatalogRepository(connectionString));
            services.AddSingleton<IMistnostRepository, MistnostRepository>(provider =>
                new MistnostRepository(connectionString));
            services.AddSingleton<IPriponaRepository, PriponaRepository>(provider =>
                new PriponaRepository(connectionString));
            services.AddSingleton<IDoctorInfoRepository, DoctorInfoRepository>(provider =>
                new DoctorInfoRepository(connectionString));
            services.AddSingleton<INavstevaDoctorViewRepository, NavstevaDoctorViewRepository>(provider =>
                new NavstevaDoctorViewRepository(connectionString));

            // Register services
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<IPatientContextService, PatientContextService>();
            services.AddSingleton<IDoctorContextService, DoctorContextService>();
            // Add other services if needed

            // Register ViewModels
            services.AddTransient<AuthVM>();
            services.AddTransient<DoctorsVM>();
            services.AddTransient<MainDoctorVM>();
            services.AddTransient<PatientsVM>();
            services.AddTransient<AdminVM>();
            services.AddTransient<AppointmentsVM>();
            services.AddTransient<PAppointmentsVM>();
            services.AddTransient<PSettingsVM>();
            services.AddTransient<EditPatientVM>();
            services.AddTransient<DoctorsListVM>();
            services.AddTransient<NewUsersVM>();
            services.AddTransient<AddAddressVM>();
            services.AddTransient<AddPositionVM>();
            services.AddTransient<NewEmployeeVM>();
            services.AddTransient<NewPatientVM>();
            services.AddTransient<TabItemVM>();
            services.AddTransient<AssignAppointmentVM>();
            services.AddTransient<UpdateAppointmentVM>();
            services.AddTransient<DDiagnosesVM>();
            services.AddTransient<AssignDiagnosisVM>();
            services.AddTransient<DSettingsVM>();
            services.AddTransient<SimulateVM>();
            services.AddTransient<AllTablesVM>();
            services.AddTransient<SystemCatalogVM>();
            services.AddTransient<LogVM>();
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
            services.AddTransient<EditPatientWindow>();
            services.AddTransient<PSettingsView>();
            services.AddTransient<DoctorsListView>();
            services.AddTransient<AssignAppointmentView>();
            services.AddTransient<UpdateAppointmentWindow>();
            services.AddTransient<AssignDiagnosisWindow>();
            services.AddTransient<DDiagnosesView>();
            services.AddTransient<DSettingsView>();
            services.AddTransient<SimulateView>();
            services.AddTransient<AllTablesView>();
            services.AddTransient<SystemCatalogView>();
            services.AddTransient<LogView>();
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