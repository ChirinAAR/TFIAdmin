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
        private string _estadoRedSensores = "Red telemétrica Agro-NOA en línea • 4/4 Estaciones operativas • Datos en tiempo real";

        public ClimaAlertasViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Estaciones = new ObservableCollection<EstacionClimatica>();
            PronosticoExtendido = new ObservableCollection<PronosticoDia>();

            SeleccionarEstacionCommand = new RelayCommand(p =>
            {
                if (p is EstacionClimatica est)
                {
                    EstacionSeleccionada = est;
                }
            });

            ActualizarDatosCommand = new RelayCommand(CargarDatos);

            CargarDatos();
        }

        public ObservableCollection<EstacionClimatica> Estaciones { get; }
        public ObservableCollection<PronosticoDia> PronosticoExtendido { get; }

        public EstacionClimatica? EstacionSeleccionada
        {
            get => _estacionSeleccionada;
            set
            {
                if (SetProperty(ref _estacionSeleccionada, value))
                {
                    ActualizarPronostico();
                }
            }
        }

        public string EstadoRedSensores
        {
            get => _estadoRedSensores;
            set => SetProperty(ref _estadoRedSensores, value);
        }

        public ICommand SeleccionarEstacionCommand { get; }
        public ICommand ActualizarDatosCommand { get; }

        public void CargarDatos()
        {
            var selId = EstacionSeleccionada?.Id ?? 1;

            Estaciones.Clear();
            foreach (var est in _agroService.ObtenerEstacionesClima())
            {
                Estaciones.Add(est);
            }

            EstacionSeleccionada = Estaciones.FirstOrDefault(e => e.Id == selId) ?? Estaciones.FirstOrDefault();
            ActualizarPronostico();
        }

        private void ActualizarPronostico()
        {
            PronosticoExtendido.Clear();
            if (EstacionSeleccionada?.PronosticoExtendido != null)
            {
                foreach (var dia in EstacionSeleccionada.PronosticoExtendido)
                {
                    PronosticoExtendido.Add(dia);
                }
            }
        }
    }
}
