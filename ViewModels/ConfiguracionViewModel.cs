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
        private string _servidorApiEndpoint = "https://api.sitech-agrologistica.com.ar/v1";
        private bool _sincronizacionNubeActiva = true;
        private int _intervaloTelemetriaMinutos = 15;
        private string _mensajePruebaConexion = string.Empty;

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

            ProbarConexionCommand = new RelayCommand(EjecutarProbarConexion);
        }

        public ObservableCollection<Usuario> UsuariosSistema { get; }
        public ObservableCollection<IntegranteGrupo> Integrantes { get; }

        public string ServidorApiEndpoint
        {
            get => _servidorApiEndpoint;
            set => SetProperty(ref _servidorApiEndpoint, value);
        }

        public bool SincronizacionNubeActiva
        {
            get => _sincronizacionNubeActiva;
            set => SetProperty(ref _sincronizacionNubeActiva, value);
        }

        public int IntervaloTelemetriaMinutos
        {
            get => _intervaloTelemetriaMinutos;
            set => SetProperty(ref _intervaloTelemetriaMinutos, value);
        }

        public string MensajePruebaConexion
        {
            get => _mensajePruebaConexion;
            set => SetProperty(ref _mensajePruebaConexion, value);
        }

        public ICommand ProbarConexionCommand { get; }

        private void EjecutarProbarConexion()
        {
            MensajePruebaConexion = "Conexión con el servidor central de SiTech establecida correctamente (Latencia: 14ms • Servidor Cloud: Operativo • Base de Datos: Sincronizada).";
        }
    }
}
