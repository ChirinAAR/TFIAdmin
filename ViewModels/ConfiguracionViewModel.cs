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
        public string Legajo { get; set; } = string.Empty;
    }

    public class ConfiguracionViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private string _servidorApiEndpoint = "https://api.sitech-agrologistica.com.ar/v1";
        private bool _modoSimulacionActivo = true;
        private int _intervaloTelemetriaMinutos = 15;
        private string _mensajePruebaConexion = string.Empty;

        public ConfiguracionViewModel(IAuthService authService)
        {
            _authService = authService;

            UsuariosSistema = new ObservableCollection<Usuario>(_authService.ObtenerUsuariosDemostracion());

            Integrantes = new ObservableCollection<IntegranteGrupo>
            {
                new IntegranteGrupo { Nombre = "Abregu Rey Emiliano Jose", Legajo = "57954" },
                new IntegranteGrupo { Nombre = "Bugeau Valentina", Legajo = "53133" },
                new IntegranteGrupo { Nombre = "Bulacio Daniel Simon", Legajo = "57124" },
                new IntegranteGrupo { Nombre = "Gallardo Piorno Geronimo", Legajo = "56313" },
                new IntegranteGrupo { Nombre = "Gil Nohra Kamila Jinette", Legajo = "57151" },
                new IntegranteGrupo { Nombre = "Juarez Julio Tobias", Legajo = "57424" },
                new IntegranteGrupo { Nombre = "Schedan Paula", Legajo = "56201" }
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

        public bool ModoSimulacionActivo
        {
            get => _modoSimulacionActivo;
            set => SetProperty(ref _modoSimulacionActivo, value);
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
            MensajePruebaConexion = "Conexión a la capa de servicios SiTech verificada con éxito (Latencia: 14ms - Capa de Mock/Dominio lista para backend REST).";
        }
    }
}
