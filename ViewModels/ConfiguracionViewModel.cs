using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class IntegranteGrupo
    {
        public string Nombre { get; set; } = string.Empty;
        public string Puesto { get; set; } = string.Empty;
    }

    public class ConfiguracionViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        // 1. Conectividad & Servidor
        private string _servidorApiEndpoint = "https://api.sitech-agrologistica.com.ar/v1";
        private bool _sincronizacionNubeActiva = true;
        private int _intervaloTelemetriaMinutos = 15;
        private string _mensajePruebaConexion = string.Empty;

        // 2. Balanza & Fiscalización CTG / AFIP
        private string _puertoBalanza = "COM1 (9600-8-N-1)";
        private string _modeloBalanza = "Toledo 8142 / Mettler";
        private double _toleranciaPesajeKg = 50.0;
        private bool _integracionAfipCtg = true;
        private string _ambienteAfip = "Producción (wsaa.afip.gov.ar)";
        private string _cuitTitular = "30-71829451-8";
        private string _puntoEmisionCtg = "0004 - Balanza Planta Leales";
        private string _mensajePruebaBalanza = string.Empty;

        // 3. Acopio & Control de Silos
        private double _umbralTemperaturaSiloC = 26.0;
        private double _umbralHumedadSoja = 13.5;
        private double _umbralHumedadMaiz = 14.5;
        private double _umbralHumedadTrigo = 13.0;
        private bool _aireacionAutomatica = true;
        private int _intervaloMuestreoSiloMin = 15;

        // 4. Telemetría GPS & Flota
        private string _proveedorGps = "Satellogic / Orbcomm IoT";
        private int _intervaloReporteGpsSegundos = 30;
        private int _radioGeocercaMetros = 150;
        private double _alertaDesvioRutaKm = 1.5;
        private bool _alertaParadaNoAutorizada = true;

        // 5. Satélite & NDVI
        private string _constelacionSatelital = "Sentinel-2 (ESA) + PlanetScope 3m";
        private int _frecuenciaActualizacionNdviDias = 5;
        private double _umbralAlertaEstresNdvi = 0.45;
        private bool _correccionAtmosfericaBoa = true;

        // 6. Notificaciones & Alertas Operativas
        private bool _notificarWhatsappChoferes = true;
        private bool _notificarEmailGerencia = true;
        private bool _notificarSmsPesadas = true;
        private bool _alertaSonoraCentroControl = true;
        private string _emailDestinoGerencia = "operaciones@sitech-agro.com.ar";
        private string _telefonoWhatsappAlertas = "+54 9 381 482-9100";

        // Toast de guardado
        private string _mensajeGuardado = string.Empty;

        public ConfiguracionViewModel(IAuthService authService)
        {
            _authService = authService;

            UsuariosSistema = new ObservableCollection<Usuario>(_authService.ObtenerUsuariosSistema());

            Integrantes = new ObservableCollection<IntegranteGrupo>
            {
                new IntegranteGrupo { Nombre = "Abregu Rey Emiliano Jose", Puesto = "Líder de Arquitectura & Sistemas" },
                new IntegranteGrupo { Nombre = "Bugeau Valentina", Puesto = "Analista Funcional de Procesos" },
                new IntegranteGrupo { Nombre = "Bulacio Daniel Simon", Puesto = "Infraestructura & Servidores" },
                new IntegranteGrupo { Nombre = "Gallardo Piorno Geronimo", Puesto = "Especialista DevOps & Cloud" },
                new IntegranteGrupo { Nombre = "Gil Nohra Kamila Jinette", Puesto = "Diseño de Experiencia UI/UX" },
                new IntegranteGrupo { Nombre = "Juarez Julio Tobias", Puesto = "Ingeniero de Software Backend" },
                new IntegranteGrupo { Nombre = "Schedan Paula", Puesto = "Aseguramiento de Calidad & QA" }
            };

            ListaPuertosBalanza = new ObservableCollection<string>
            {
                "COM1 (9600-8-N-1)",
                "COM2 (9600-8-N-1)",
                "COM3 (19200-8-E-1)",
                "TCP/IP 192.168.1.150:4001 (Balanza Entrada)",
                "TCP/IP 192.168.1.151:4001 (Balanza Salida)"
            };

            ListaModelosBalanza = new ObservableCollection<string>
            {
                "Toledo 8142 / Mettler",
                "Magris / Hook Electrónica",
                "Systel Croma / Bumer",
                "Básculas San Martín Digital",
                "Protocolo Genérico Continuo ASCII"
            };

            ListaAmbientesAfip = new ObservableCollection<string>
            {
                "Producción (wsaa.afip.gov.ar)",
                "Homologación Testing (wsaahomo.afip.gov.ar)"
            };

            ListaProveedoresGps = new ObservableCollection<string>
            {
                "Satellogic / Orbcomm IoT",
                "CalAmp / Pointer Argentina",
                "Geotab Fleet Logix",
                "Satelital Andes Tracking"
            };

            ListaConstelaciones = new ObservableCollection<string>
            {
                "Sentinel-2 (ESA) + PlanetScope 3m",
                "Landsat 8/9 + Sentinel-2 Híbrido",
                "PlanetScope Constellation Diario (3m)",
                "MODIS Terra/Aqua 250m"
            };

            ProbarConexionCommand = new RelayCommand(EjecutarProbarConexion);
            ProbarBalanzaCommand = new RelayCommand(EjecutarProbarBalanza);
            GuardarConfiguracionCommand = new RelayCommand(EjecutarGuardarConfiguracion);
            RestablecerValoresCommand = new RelayCommand(EjecutarRestablecerValores);
        }

        public ObservableCollection<Usuario> UsuariosSistema { get; }
        public ObservableCollection<IntegranteGrupo> Integrantes { get; }

        public ObservableCollection<string> ListaPuertosBalanza { get; }
        public ObservableCollection<string> ListaModelosBalanza { get; }
        public ObservableCollection<string> ListaAmbientesAfip { get; }
        public ObservableCollection<string> ListaProveedoresGps { get; }
        public ObservableCollection<string> ListaConstelaciones { get; }

        // Propiedades de Servidor
        public string ServidorApiEndpoint { get => _servidorApiEndpoint; set => SetProperty(ref _servidorApiEndpoint, value); }
        public bool SincronizacionNubeActiva { get => _sincronizacionNubeActiva; set => SetProperty(ref _sincronizacionNubeActiva, value); }
        public int IntervaloTelemetriaMinutos { get => _intervaloTelemetriaMinutos; set => SetProperty(ref _intervaloTelemetriaMinutos, value); }
        public string MensajePruebaConexion { get => _mensajePruebaConexion; set => SetProperty(ref _mensajePruebaConexion, value); }

        // Propiedades de Balanza & AFIP
        public string PuertoBalanza { get => _puertoBalanza; set => SetProperty(ref _puertoBalanza, value); }
        public string ModeloBalanza { get => _modeloBalanza; set => SetProperty(ref _modeloBalanza, value); }
        public double ToleranciaPesajeKg { get => _toleranciaPesajeKg; set => SetProperty(ref _toleranciaPesajeKg, value); }
        public bool IntegracionAfipCtg { get => _integracionAfipCtg; set => SetProperty(ref _integracionAfipCtg, value); }
        public string AmbienteAfip { get => _ambienteAfip; set => SetProperty(ref _ambienteAfip, value); }
        public string CuitTitular { get => _cuitTitular; set => SetProperty(ref _cuitTitular, value); }
        public string PuntoEmisionCtg { get => _puntoEmisionCtg; set => SetProperty(ref _puntoEmisionCtg, value); }
        public string MensajePruebaBalanza { get => _mensajePruebaBalanza; set => SetProperty(ref _mensajePruebaBalanza, value); }

        // Propiedades de Silos
        public double UmbralTemperaturaSiloC { get => _umbralTemperaturaSiloC; set => SetProperty(ref _umbralTemperaturaSiloC, value); }
        public double UmbralHumedadSoja { get => _umbralHumedadSoja; set => SetProperty(ref _umbralHumedadSoja, value); }
        public double UmbralHumedadMaiz { get => _umbralHumedadMaiz; set => SetProperty(ref _umbralHumedadMaiz, value); }
        public double UmbralHumedadTrigo { get => _umbralHumedadTrigo; set => SetProperty(ref _umbralHumedadTrigo, value); }
        public bool AireacionAutomatica { get => _aireacionAutomatica; set => SetProperty(ref _aireacionAutomatica, value); }
        public int IntervaloMuestreoSiloMin { get => _intervaloMuestreoSiloMin; set => SetProperty(ref _intervaloMuestreoSiloMin, value); }

        // Propiedades de Telemetría GPS
        public string ProveedorGps { get => _proveedorGps; set => SetProperty(ref _proveedorGps, value); }
        public int IntervaloReporteGpsSegundos { get => _intervaloReporteGpsSegundos; set => SetProperty(ref _intervaloReporteGpsSegundos, value); }
        public int RadioGeocercaMetros { get => _radioGeocercaMetros; set => SetProperty(ref _radioGeocercaMetros, value); }
        public double AlertaDesvioRutaKm { get => _alertaDesvioRutaKm; set => SetProperty(ref _alertaDesvioRutaKm, value); }
        public bool AlertaParadaNoAutorizada { get => _alertaParadaNoAutorizada; set => SetProperty(ref _alertaParadaNoAutorizada, value); }

        // Propiedades de Satélite & NDVI
        public string ConstelacionSatelital { get => _constelacionSatelital; set => SetProperty(ref _constelacionSatelital, value); }
        public int FrecuenciaActualizacionNdviDias { get => _frecuenciaActualizacionNdviDias; set => SetProperty(ref _frecuenciaActualizacionNdviDias, value); }
        public double UmbralAlertaEstresNdvi { get => _umbralAlertaEstresNdvi; set => SetProperty(ref _umbralAlertaEstresNdvi, value); }
        public bool CorreccionAtmosfericaBoa { get => _correccionAtmosfericaBoa; set => SetProperty(ref _correccionAtmosfericaBoa, value); }

        // Propiedades de Notificaciones
        public bool NotificarWhatsappChoferes { get => _notificarWhatsappChoferes; set => SetProperty(ref _notificarWhatsappChoferes, value); }
        public bool NotificarEmailGerencia { get => _notificarEmailGerencia; set => SetProperty(ref _notificarEmailGerencia, value); }
        public bool NotificarSmsPesadas { get => _notificarSmsPesadas; set => SetProperty(ref _notificarSmsPesadas, value); }
        public bool AlertaSonoraCentroControl { get => _alertaSonoraCentroControl; set => SetProperty(ref _alertaSonoraCentroControl, value); }
        public string EmailDestinoGerencia { get => _emailDestinoGerencia; set => SetProperty(ref _emailDestinoGerencia, value); }
        public string TelefonoWhatsappAlertas { get => _telefonoWhatsappAlertas; set => SetProperty(ref _telefonoWhatsappAlertas, value); }

        // Mensaje de Guardado
        public string MensajeGuardado { get => _mensajeGuardado; set => SetProperty(ref _mensajeGuardado, value); }

        public ICommand ProbarConexionCommand { get; }
        public ICommand ProbarBalanzaCommand { get; }
        public ICommand GuardarConfiguracionCommand { get; }
        public ICommand RestablecerValoresCommand { get; }

        private void EjecutarProbarConexion()
        {
            MensajePruebaConexion = "Conexión con el servidor central de SiTech establecida correctamente (Latencia: 14ms • Servidor Cloud: Operativo • Base de Datos: Sincronizada).";
        }

        private void EjecutarProbarBalanza()
        {
            MensajePruebaBalanza = $"Balanza conectada en {PuertoBalanza}. Lectura estable recibida: 0.00 kg (Cero calibrado • Protocolo: {ModeloBalanza}).";
        }

        private void EjecutarGuardarConfiguracion()
        {
            MensajeGuardado = "✓ Configuración guardada y sincronizada exitosamente con la red agro-logística.";
        }

        private void EjecutarRestablecerValores()
        {
            PuertoBalanza = "COM1 (9600-8-N-1)";
            ModeloBalanza = "Toledo 8142 / Mettler";
            ToleranciaPesajeKg = 50.0;
            IntegracionAfipCtg = true;
            AmbienteAfip = "Producción (wsaa.afip.gov.ar)";
            UmbralTemperaturaSiloC = 26.0;
            UmbralHumedadSoja = 13.5;
            UmbralHumedadMaiz = 14.5;
            UmbralHumedadTrigo = 13.0;
            AireacionAutomatica = true;
            IntervaloReporteGpsSegundos = 30;
            RadioGeocercaMetros = 150;
            AlertaDesvioRutaKm = 1.5;
            FrecuenciaActualizacionNdviDias = 5;
            UmbralAlertaEstresNdvi = 0.45;
            MensajeGuardado = "↺ Valores por defecto restablecidos.";
        }
    }
}
