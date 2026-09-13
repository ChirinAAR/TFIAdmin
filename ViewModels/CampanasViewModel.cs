using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class CampanasViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private CampanaProyecto? _campanaSeleccionada;
        private bool _mostrarDialogoNuevo;

        // Propiedades nueva campaña
        private string _nuevoNombre = "Campaña Trigo Fina 2026/27";
        private string _nuevoTipo = "Fina";
        private string _nuevoResponsable = "Ing. Germán Paz";
        private double _nuevasHectareas = 4200;
        private double _nuevasToneladas = 12600;
        private double _nuevoPresupuesto = 650000;

        public CampanasViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Campanas = new ObservableCollection<CampanaProyecto>();

            AbrirNuevoCommand = new RelayCommand(() => MostrarDialogoNuevo = true);
            CancelarNuevoCommand = new RelayCommand(() => MostrarDialogoNuevo = false);
            GuardarNuevoCommand = new RelayCommand(EjecutarGuardarNuevo);

            CargarCampanas();
        }

        public ObservableCollection<CampanaProyecto> Campanas { get; }

        public CampanaProyecto? CampanaSeleccionada
        {
            get => _campanaSeleccionada;
            set => SetProperty(ref _campanaSeleccionada, value);
        }

        public bool MostrarDialogoNuevo
        {
            get => _mostrarDialogoNuevo;
            set => SetProperty(ref _mostrarDialogoNuevo, value);
        }

        public string NuevoNombre { get => _nuevoNombre; set => SetProperty(ref _nuevoNombre, value); }
        public string NuevoTipo { get => _nuevoTipo; set => SetProperty(ref _nuevoTipo, value); }
        public string NuevoResponsable { get => _nuevoResponsable; set => SetProperty(ref _nuevoResponsable, value); }
        public double NuevasHectareas { get => _nuevasHectareas; set => SetProperty(ref _nuevasHectareas, value); }
        public double NuevasToneladas { get => _nuevasToneladas; set => SetProperty(ref _nuevasToneladas, value); }
        public double NuevoPresupuesto { get => _nuevoPresupuesto; set => SetProperty(ref _nuevoPresupuesto, value); }

        public ICommand AbrirNuevoCommand { get; }
        public ICommand CancelarNuevoCommand { get; }
        public ICommand GuardarNuevoCommand { get; }

        public void CargarCampanas()
        {
            Campanas.Clear();
            foreach (var c in _agroService.ObtenerCampanas())
            {
                Campanas.Add(c);
            }
            if (CampanaSeleccionada == null && Campanas.Any())
                CampanaSeleccionada = Campanas.First();
        }

        private void EjecutarGuardarNuevo()
        {
            if (string.IsNullOrWhiteSpace(NuevoNombre)) return;

            var camp = new CampanaProyecto
            {
                Nombre = NuevoNombre,
                TipoCiclo = NuevoTipo,
                Codigo = $"CAMP-{DateTime.Now.Year}-{NuevoTipo.ToUpper()[..Math.Min(4, NuevoTipo.Length)]}",
                FechaInicio = DateTime.Now,
                FechaEstimadaCierre = DateTime.Now.AddMonths(6),
                ResponsableGeneral = NuevoResponsable,
                HectareasPlanificadas = NuevasHectareas,
                ToneladasObjetivo = NuevasToneladas,
                PresupuestoEstimadoUsd = NuevoPresupuesto,
                AvancePorcentaje = 0,
                Estado = "Planificada"
            };

            _agroService.AgregarCampana(camp);
            CargarCampanas();
            CampanaSeleccionada = camp;
            MostrarDialogoNuevo = false;
        }
    }
}
