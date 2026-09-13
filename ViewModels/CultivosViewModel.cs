using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class CultivosViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;

        private string _filtroCultivo = "Todos";
        private string _textoBusqueda = string.Empty;
        private CultivoLote? _loteSeleccionado;
        private bool _mostrarFormularioNuevo;

        // Propiedades de nuevo lote
        private string _nuevoNombre = string.Empty;
        private string _nuevaFinca = string.Empty;
        private string _nuevaZona = "Leales, Tucumán";
        private string _nuevoCultivo = "Soja";
        private string _nuevaVariedad = string.Empty;
        private double _nuevasHectareas = 150;
        private string _nuevoEstadoFenologico = "Vegetativo";

        public CultivosViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;

            Lotes = new ObservableCollection<CultivoLote>();
            CultivosDisponibles = new ObservableCollection<string>
            {
                "Todos", "Soja", "Maíz", "Limón", "Caña de Azúcar", "Poroto"
            };

            AbrirNuevoLoteCommand = new RelayCommand(() => MostrarFormularioNuevo = true);
            CancelarNuevoLoteCommand = new RelayCommand(() => MostrarFormularioNuevo = false);
            GuardarNuevoLoteCommand = new RelayCommand(EjecutarGuardarNuevoLote);
            FiltrarCommand = new RelayCommand(ActualizarFiltro);

            CargarLotes();
        }

        public ObservableCollection<CultivoLote> Lotes { get; }
        public ObservableCollection<string> CultivosDisponibles { get; }

        public string FiltroCultivo
        {
            get => _filtroCultivo;
            set
            {
                if (SetProperty(ref _filtroCultivo, value))
                    ActualizarFiltro();
            }
        }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                    ActualizarFiltro();
            }
        }

        public CultivoLote? LoteSeleccionado
        {
            get => _loteSeleccionado;
            set => SetProperty(ref _loteSeleccionado, value);
        }

        public bool MostrarFormularioNuevo
        {
            get => _mostrarFormularioNuevo;
            set => SetProperty(ref _mostrarFormularioNuevo, value);
        }

        public string NuevoNombre { get => _nuevoNombre; set => SetProperty(ref _nuevoNombre, value); }
        public string NuevaFinca { get => _nuevaFinca; set => SetProperty(ref _nuevaFinca, value); }
        public string NuevaZona { get => _nuevaZona; set => SetProperty(ref _nuevaZona, value); }
        public string NuevoCultivo { get => _nuevoCultivo; set => SetProperty(ref _nuevoCultivo, value); }
        public string NuevaVariedad { get => _nuevaVariedad; set => SetProperty(ref _nuevaVariedad, value); }
        public double NuevasHectareas { get => _nuevasHectareas; set => SetProperty(ref _nuevasHectareas, value); }
        public string NuevoEstadoFenologico { get => _nuevoEstadoFenologico; set => SetProperty(ref _nuevoEstadoFenologico, value); }

        public ICommand AbrirNuevoLoteCommand { get; }
        public ICommand CancelarNuevoLoteCommand { get; }
        public ICommand GuardarNuevoLoteCommand { get; }
        public ICommand FiltrarCommand { get; }

        public void CargarLotes()
        {
            ActualizarFiltro();
            if (LoteSeleccionado == null && Lotes.Any())
                LoteSeleccionado = Lotes.First();
        }

        private void ActualizarFiltro()
        {
            var consulta = _agroService.ObtenerLotes().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FiltroCultivo) && FiltroCultivo != "Todos")
            {
                consulta = consulta.Where(l => l.TipoCultivo.Equals(FiltroCultivo, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                consulta = consulta.Where(l =>
                    l.Nombre.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    l.CodigoLote.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    l.ZonaUbicacion.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));
            }

            Lotes.Clear();
            foreach (var lote in consulta)
            {
                Lotes.Add(lote);
            }
        }

        private void EjecutarGuardarNuevoLote()
        {
            if (string.IsNullOrWhiteSpace(NuevoNombre)) return;

            var nuevo = new CultivoLote
            {
                Nombre = NuevoNombre,
                FincaEstablecimiento = string.IsNullOrWhiteSpace(NuevaFinca) ? "Establecimiento NOA Central" : NuevaFinca,
                ZonaUbicacion = NuevaZona,
                TipoCultivo = NuevoCultivo,
                VariedadHibrido = string.IsNullOrWhiteSpace(NuevaVariedad) ? "Híbrido Certificado" : NuevaVariedad,
                Hectareas = NuevasHectareas,
                EstadoFenologico = NuevoEstadoFenologico,
                HumedadSueloPorcentaje = 65.0,
                IndiceVerdeNDVI = 0.75,
                EstadoSanitario = "Óptimo",
                FechaSiembra = DateTime.Now.AddMonths(-2),
                FechaEstimadaCosecha = DateTime.Now.AddMonths(2),
                RindeEsperadoTnHa = 3.5,
                NotasObservaciones = "Lote incorporado al monitoreo satelital SiTech."
            };

            _agroService.AgregarLote(nuevo);
            ActualizarFiltro();
            LoteSeleccionado = nuevo;
            MostrarFormularioNuevo = false;

            // Limpiar campos
            NuevoNombre = string.Empty;
            NuevaFinca = string.Empty;
        }
    }
}
