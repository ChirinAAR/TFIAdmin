using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class DashboardViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private readonly Action<string> _navegarAModulo;
        private string _filtroArea = "Todas";

        public DashboardViewModel(IAgroDataService agroService, Action<string> navegarAModulo)
        {
            _agroService = agroService;
            _navegarAModulo = navegarAModulo;

            Kpis = new ObservableCollection<IndicadorKpi>();
            ViajesEnCurso = new ObservableCollection<TransporteViaje>();
            AlertasUrgentes = new ObservableCollection<AlertaMeteorologica>();
            FiltrosDisponibles = new ObservableCollection<string>
            {
                "Todas", "Cultivos", "Cosecha", "Transporte", "Acopio", "Clima"
            };

            NavegarCommand = new RelayCommand(p =>
            {
                if (p is string destino)
                    _navegarAModulo(destino);
            });

            NavegarAModuloAlertaCommand = new RelayCommand(p =>
            {
                if (p is AlertaMeteorologica alerta)
                    _navegarAModulo(alerta.ModuloOrigen);
                else if (p is string destino)
                    _navegarAModulo(destino);
            });

            FiltrarAreaCommand = new RelayCommand(p =>
            {
                if (p is string area)
                {
                    FiltroArea = area;
                }
            });

            ResolverAlertaCommand = new RelayCommand(p => EjecutarResolverAlerta(p as AlertaMeteorologica));
            ActualizarCommand = new RelayCommand(ActualizarDatos);

            ActualizarDatos();
        }

        public ObservableCollection<IndicadorKpi> Kpis { get; }
        public ObservableCollection<TransporteViaje> ViajesEnCurso { get; }
        public ObservableCollection<AlertaMeteorologica> AlertasUrgentes { get; }
        public ObservableCollection<string> FiltrosDisponibles { get; }

        public string FiltroArea
        {
            get => _filtroArea;
            set
            {
                if (SetProperty(ref _filtroArea, value))
                {
                    RecargarAlertas();
                }
            }
        }

        public int TotalAlertasActivas => _agroService.ObtenerAlertas().Count(a => a.Activa);
        public int TotalAlertasCriticas => _agroService.ObtenerAlertas().Count(a => a.Activa && a.NivelSeveridad == "Crítica");
        public int TotalAlertasAltas => _agroService.ObtenerAlertas().Count(a => a.Activa && a.NivelSeveridad == "Alta");

        public ICommand NavegarCommand { get; }
        public ICommand NavegarAModuloAlertaCommand { get; }
        public ICommand FiltrarAreaCommand { get; }
        public ICommand ResolverAlertaCommand { get; }
        public ICommand ActualizarCommand { get; }

        public void ActualizarDatos()
        {
            Kpis.Clear();
            foreach (var kpi in _agroService.ObtenerKpisPrincipales())
            {
                Kpis.Add(kpi);
            }

            ViajesEnCurso.Clear();
            foreach (var viaje in _agroService.ObtenerViajesTransporte().Take(4))
            {
                ViajesEnCurso.Add(viaje);
            }

            RecargarAlertas();
        }

        private void RecargarAlertas()
        {
            AlertasUrgentes.Clear();
            var todas = _agroService.ObtenerAlertas().Where(a => a.Activa);

            if (_filtroArea != "Todas")
            {
                todas = todas.Where(a => a.ModuloOrigen.Equals(_filtroArea, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var alerta in todas.OrderByDescending(a => a.NivelSeveridad == "Crítica")
                                        .ThenByDescending(a => a.NivelSeveridad == "Alta")
                                        .ThenByDescending(a => a.FechaEmision))
            {
                AlertasUrgentes.Add(alerta);
            }

            OnPropertyChanged(nameof(TotalAlertasActivas));
            OnPropertyChanged(nameof(TotalAlertasCriticas));
            OnPropertyChanged(nameof(TotalAlertasAltas));
        }

        private void EjecutarResolverAlerta(AlertaMeteorologica? alerta)
        {
            if (alerta != null)
            {
                _agroService.ResolverAlerta(alerta.Id);
                RecargarAlertas();
            }
        }
    }
}
