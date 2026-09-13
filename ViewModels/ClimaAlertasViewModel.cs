using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class ClimaAlertasViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private EstacionClimatica? _estacionSeleccionada;
        private AlertaMeteorologica? _alertaSeleccionada;
        private bool _mostrarDialogoNuevaAlerta;

        // Propiedades de nueva alerta
        private string _nuevoTitulo = "Alerta Preventiva de Tormenta";
        private string _nuevoTipo = "Tormenta Fuerte";
        private string _nuevaSeveridad = "Media";
        private string _nuevaZona = "Leales y Cruz Alta - Tucumán";
        private string _nuevaDescripcion = "Probabilidad de lluvias con vientos moderados a fuertes.";
        private string _nuevaRecomendacion = "Verificar lonas de acopio y monitorear caminos de ripio.";

        public ClimaAlertasViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Estaciones = new ObservableCollection<EstacionClimatica>();
            Alertas = new ObservableCollection<AlertaMeteorologica>();

            AbrirNuevaAlertaCommand = new RelayCommand(() => MostrarDialogoNuevaAlerta = true);
            CancelarNuevaAlertaCommand = new RelayCommand(() => MostrarDialogoNuevaAlerta = false);
            GuardarNuevaAlertaCommand = new RelayCommand(EjecutarGuardarNuevaAlerta);
            ResolverAlertaCommand = new RelayCommand(p => EjecutarResolverAlerta(p as AlertaMeteorologica));

            CargarDatos();
        }

        public ObservableCollection<EstacionClimatica> Estaciones { get; }
        public ObservableCollection<AlertaMeteorologica> Alertas { get; }

        public EstacionClimatica? EstacionSeleccionada
        {
            get => _estacionSeleccionada;
            set => SetProperty(ref _estacionSeleccionada, value);
        }

        public AlertaMeteorologica? AlertaSeleccionada
        {
            get => _alertaSeleccionada;
            set => SetProperty(ref _alertaSeleccionada, value);
        }

        public bool MostrarDialogoNuevaAlerta
        {
            get => _mostrarDialogoNuevaAlerta;
            set => SetProperty(ref _mostrarDialogoNuevaAlerta, value);
        }

        public string NuevoTitulo { get => _nuevoTitulo; set => SetProperty(ref _nuevoTitulo, value); }
        public string NuevoTipo { get => _nuevoTipo; set => SetProperty(ref _nuevoTipo, value); }
        public string NuevaSeveridad { get => _nuevaSeveridad; set => SetProperty(ref _nuevaSeveridad, value); }
        public string NuevaZona { get => _nuevaZona; set => SetProperty(ref _nuevaZona, value); }
        public string NuevaDescripcion { get => _nuevaDescripcion; set => SetProperty(ref _nuevaDescripcion, value); }
        public string NuevaRecomendacion { get => _nuevaRecomendacion; set => SetProperty(ref _nuevaRecomendacion, value); }

        public ICommand AbrirNuevaAlertaCommand { get; }
        public ICommand CancelarNuevaAlertaCommand { get; }
        public ICommand GuardarNuevaAlertaCommand { get; }
        public ICommand ResolverAlertaCommand { get; }

        public void CargarDatos()
        {
            Estaciones.Clear();
            foreach (var est in _agroService.ObtenerEstacionesClima())
            {
                Estaciones.Add(est);
            }
            if (EstacionSeleccionada == null && Estaciones.Any())
                EstacionSeleccionada = Estaciones.First();

            Alertas.Clear();
            foreach (var al in _agroService.ObtenerAlertas())
            {
                Alertas.Add(al);
            }
            if (AlertaSeleccionada == null && Alertas.Any())
                AlertaSeleccionada = Alertas.First();
        }

        private void EjecutarGuardarNuevaAlerta()
        {
            if (string.IsNullOrWhiteSpace(NuevoTitulo)) return;

            var nueva = new AlertaMeteorologica
            {
                Titulo = NuevoTitulo,
                TipoAlerta = NuevoTipo,
                NivelSeveridad = NuevaSeveridad,
                ZonaAfectada = NuevaZona,
                Descripcion = NuevaDescripcion,
                RecomendacionOperativa = NuevaRecomendacion,
                FechaEmision = DateTime.Now,
                Activa = true
            };

            _agroService.AgregarAlerta(nueva);
            CargarDatos();
            AlertaSeleccionada = nueva;
            MostrarDialogoNuevaAlerta = false;
        }

        private void EjecutarResolverAlerta(AlertaMeteorologica? alerta)
        {
            if (alerta != null)
            {
                _agroService.ResolverAlerta(alerta.Id);
                CargarDatos();
            }
        }
    }
}
