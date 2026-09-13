using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class AcopioViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private AcopioSilo? _siloSeleccionado;
        private double _toneladasMovimiento = 30.0;
        private string _mensajeAccion = string.Empty;

        public AcopioViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Silos = new ObservableCollection<AcopioSilo>();

            IngresoGranosCommand = new RelayCommand(EjecutarIngresoGranos, () => SiloSeleccionado != null);
            EgresoGranosCommand = new RelayCommand(EjecutarEgresoGranos, () => SiloSeleccionado != null);
            SeleccionarSiloCommand = new RelayCommand(p =>
            {
                if (p is AcopioSilo silo)
                    SiloSeleccionado = silo;
            });

            CargarSilos();
        }

        public ObservableCollection<AcopioSilo> Silos { get; }
        public ICommand SeleccionarSiloCommand { get; }

        public AcopioSilo? SiloSeleccionado
        {
            get => _siloSeleccionado;
            set => SetProperty(ref _siloSeleccionado, value);
        }

        public double ToneladasMovimiento
        {
            get => _toneladasMovimiento;
            set => SetProperty(ref _toneladasMovimiento, value);
        }

        public string MensajeAccion
        {
            get => _mensajeAccion;
            set => SetProperty(ref _mensajeAccion, value);
        }

        public ICommand IngresoGranosCommand { get; }
        public ICommand EgresoGranosCommand { get; }

        public void CargarSilos()
        {
            int? idSeleccionado = SiloSeleccionado?.Id;
            Silos.Clear();
            foreach (var silo in _agroService.ObtenerSilos())
            {
                Silos.Add(silo);
            }

            if (idSeleccionado.HasValue)
                SiloSeleccionado = Silos.FirstOrDefault(s => s.Id == idSeleccionado.Value);

            if (SiloSeleccionado == null && Silos.Any())
                SiloSeleccionado = Silos.First();
        }

        private void EjecutarIngresoGranos()
        {
            if (SiloSeleccionado == null) return;
            _agroService.ActualizarStockSilo(SiloSeleccionado.Id, ToneladasMovimiento);
            MensajeAccion = $"Ingreso registrado: +{ToneladasMovimiento:N1} Tn en {SiloSeleccionado.Identificador}.";
            CargarSilos();
        }

        private void EjecutarEgresoGranos()
        {
            if (SiloSeleccionado == null) return;
            _agroService.ActualizarStockSilo(SiloSeleccionado.Id, -ToneladasMovimiento);
            MensajeAccion = $"Despacho registrado: -{ToneladasMovimiento:N1} Tn desde {SiloSeleccionado.Identificador}.";
            CargarSilos();
        }
    }
}
