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

        public DashboardViewModel(IAgroDataService agroService, Action<string> navegarAModulo)
        {
            _agroService = agroService;
            _navegarAModulo = navegarAModulo;

            Kpis = new ObservableCollection<IndicadorKpi>();
            ViajesEnCurso = new ObservableCollection<TransporteViaje>();
            AlertasUrgentes = new ObservableCollection<AlertaMeteorologica>();

            NavegarCommand = new RelayCommand(p =>
            {
                if (p is string destino)
                    _navegarAModulo(destino);
            });

            ActualizarDatos();
        }

        public ObservableCollection<IndicadorKpi> Kpis { get; }
        public ObservableCollection<TransporteViaje> ViajesEnCurso { get; }
        public ObservableCollection<AlertaMeteorologica> AlertasUrgentes { get; }

        public ICommand NavegarCommand { get; }

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

            AlertasUrgentes.Clear();
            foreach (var alerta in _agroService.ObtenerAlertas().Where(a => a.Activa).Take(3))
            {
                AlertasUrgentes.Add(alerta);
            }
        }
    }
}
