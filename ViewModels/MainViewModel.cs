using System;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.Services.Mock;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IAgroDataService _agroService;

        private object? _currentViewModel;
        private bool _estaAutenticado;
        private Usuario? _usuarioActual;
        private string _menuActivo = "Dashboard";

        // ViewModels cacheados
        private LoginViewModel? _loginVM;
        private DashboardViewModel? _dashboardVM;
        private CultivosViewModel? _cultivosVM;
        private CosechaViewModel? _cosechaVM;
        private TransporteViewModel? _transporteVM;
        private AcopioViewModel? _acopioVM;
        private ClimaAlertasViewModel? _climaVM;
        private TrazabilidadViewModel? _trazabilidadVM;
        private ClientesViewModel? _clientesVM;
        private CampanasViewModel? _campanasVM;
        private EmpleadosViewModel? _empleadosVM;
        private ReportesViewModel? _reportesVM;
        private ConfiguracionViewModel? _configuracionVM;

        public MainViewModel()
            : this(new AuthMockService(), new AgroMockDataService())
        {
        }

        public MainViewModel(IAuthService authService, IAgroDataService agroService)
        {
            _authService = authService;
            _agroService = agroService;

            NavegarCommand = new RelayCommand(p =>
            {
                if (p is string destino)
                    NavegarA(destino);
            });

            CerrarSesionCommand = new RelayCommand(EjecutarCerrarSesion);

            InicializarVistas();
            MostrarLogin();
        }

        public object? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public bool EstaAutenticado
        {
            get => _estaAutenticado;
            set => SetProperty(ref _estaAutenticado, value);
        }

        public Usuario? UsuarioActual
        {
            get => _usuarioActual;
            set => SetProperty(ref _usuarioActual, value);
        }

        public string MenuActivo
        {
            get => _menuActivo;
            set => SetProperty(ref _menuActivo, value);
        }

        public int CantidadAlertasActivas => _agroService.ObtenerAlertas().Count(a => a.Activa);

        public ICommand NavegarCommand { get; }
        public ICommand CerrarSesionCommand { get; }

        private void InicializarVistas()
        {
            _loginVM = new LoginViewModel(_authService, OnLoginExitoso);
            _dashboardVM = new DashboardViewModel(_agroService, NavegarA);
            _cultivosVM = new CultivosViewModel(_agroService);
            _cosechaVM = new CosechaViewModel(_agroService);
            _transporteVM = new TransporteViewModel(_agroService);
            _acopioVM = new AcopioViewModel(_agroService);
            _climaVM = new ClimaAlertasViewModel(_agroService);
            _trazabilidadVM = new TrazabilidadViewModel(_agroService);
            _clientesVM = new ClientesViewModel(_agroService);
            _campanasVM = new CampanasViewModel(_agroService);
            _empleadosVM = new EmpleadosViewModel(_agroService);
            _reportesVM = new ReportesViewModel(_agroService);
            _configuracionVM = new ConfiguracionViewModel(_authService);
        }

        private void MostrarLogin()
        {
            EstaAutenticado = false;
            UsuarioActual = null;
            CurrentViewModel = _loginVM;
        }

        private void OnLoginExitoso(Usuario usuario)
        {
            UsuarioActual = usuario;
            EstaAutenticado = true;
            NavegarA("Dashboard");
        }

        public void NavegarA(string destino)
        {
            MenuActivo = destino;
            OnPropertyChanged(nameof(CantidadAlertasActivas));

            switch (destino)
            {
                case "Dashboard":
                    _dashboardVM?.ActualizarDatos();
                    CurrentViewModel = _dashboardVM;
                    break;
                case "Cultivos":
                    _cultivosVM?.CargarLotes();
                    CurrentViewModel = _cultivosVM;
                    break;
                case "Cosecha":
                    _cosechaVM?.CargarLabores();
                    CurrentViewModel = _cosechaVM;
                    break;
                case "Transporte":
                    _transporteVM?.CargarViajes();
                    CurrentViewModel = _transporteVM;
                    break;
                case "Acopio":
                    _acopioVM?.CargarSilos();
                    CurrentViewModel = _acopioVM;
                    break;
                case "Clima":
                    _climaVM?.CargarDatos();
                    CurrentViewModel = _climaVM;
                    break;
                case "Trazabilidad":
                    _trazabilidadVM?.CargarPartidas();
                    CurrentViewModel = _trazabilidadVM;
                    break;
                case "Clientes":
                    _clientesVM?.CargarClientes();
                    CurrentViewModel = _clientesVM;
                    break;
                case "Campanas":
                    _campanasVM?.CargarCampanas();
                    CurrentViewModel = _campanasVM;
                    break;
                case "Empleados":
                    _empleadosVM?.CargarEmpleados();
                    CurrentViewModel = _empleadosVM;
                    break;
                case "Reportes":
                    CurrentViewModel = _reportesVM;
                    break;
                case "Configuracion":
                    CurrentViewModel = _configuracionVM;
                    break;
                default:
                    CurrentViewModel = _dashboardVM;
                    break;
            }
        }

        private void EjecutarCerrarSesion()
        {
            _authService.CerrarSesion();
            MostrarLogin();
        }
    }
}
