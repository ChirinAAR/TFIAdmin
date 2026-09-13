using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class TransporteViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private string _filtroEstado = "Todos";
        private TransporteViaje? _viajeSeleccionado;
        private bool _mostrarDialogoNuevoDespacho;

        // Propiedades de nuevo viaje
        private string _nuevoChofer = "Marcos Argañaraz";
        private string _nuevaPatente = "AF-550-ZZ";
        private string _nuevoOrigen = "Lote El Manantial Norte";
        private string _nuevoDestino = "Planta de Acopio Central SiTech";
        private string _nuevoGrano = "Soja";
        private double _nuevasToneladas = 31.5;

        public TransporteViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Viajes = new ObservableCollection<TransporteViaje>();
            EstadosDisponibles = new ObservableCollection<string>
            {
                "Todos", "En Carga", "En Balanza Origen", "En Tránsito", "Descargado"
            };

            AbrirNuevoDespachoCommand = new RelayCommand(() => MostrarDialogoNuevoDespacho = true);
            CancelarNuevoDespachoCommand = new RelayCommand(() => MostrarDialogoNuevoDespacho = false);
            GuardarNuevoDespachoCommand = new RelayCommand(EjecutarGuardarNuevoDespacho);
            AvanzarEstadoCommand = new RelayCommand(EjecutarAvanzarEstado, () => ViajeSeleccionado != null);

            CargarViajes();
        }

        public ObservableCollection<TransporteViaje> Viajes { get; }
        public ObservableCollection<string> EstadosDisponibles { get; }

        public string FiltroEstado
        {
            get => _filtroEstado;
            set
            {
                if (SetProperty(ref _filtroEstado, value))
                    CargarViajes();
            }
        }

        public TransporteViaje? ViajeSeleccionado
        {
            get => _viajeSeleccionado;
            set => SetProperty(ref _viajeSeleccionado, value);
        }

        public bool MostrarDialogoNuevoDespacho
        {
            get => _mostrarDialogoNuevoDespacho;
            set => SetProperty(ref _mostrarDialogoNuevoDespacho, value);
        }

        public string NuevoChofer { get => _nuevoChofer; set => SetProperty(ref _nuevoChofer, value); }
        public string NuevaPatente { get => _nuevaPatente; set => SetProperty(ref _nuevaPatente, value); }
        public string NuevoOrigen { get => _nuevoOrigen; set => SetProperty(ref _nuevoOrigen, value); }
        public string NuevoDestino { get => _nuevoDestino; set => SetProperty(ref _nuevoDestino, value); }
        public string NuevoGrano { get => _nuevoGrano; set => SetProperty(ref _nuevoGrano, value); }
        public double NuevasToneladas { get => _nuevasToneladas; set => SetProperty(ref _nuevasToneladas, value); }

        public ICommand AbrirNuevoDespachoCommand { get; }
        public ICommand CancelarNuevoDespachoCommand { get; }
        public ICommand GuardarNuevoDespachoCommand { get; }
        public ICommand AvanzarEstadoCommand { get; }

        public void CargarViajes()
        {
            var consulta = _agroService.ObtenerViajesTransporte().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(FiltroEstado) && FiltroEstado != "Todos")
            {
                consulta = consulta.Where(v => v.Estado.Equals(FiltroEstado, StringComparison.OrdinalIgnoreCase));
            }

            Viajes.Clear();
            foreach (var viaje in consulta)
            {
                Viajes.Add(viaje);
            }

            if (ViajeSeleccionado == null && Viajes.Any())
                ViajeSeleccionado = Viajes.First();
        }

        private void EjecutarAvanzarEstado()
        {
            if (ViajeSeleccionado == null) return;

            string proximoEstado;
            double proximoProgreso;

            switch (ViajeSeleccionado.Estado)
            {
                case "En Carga":
                    proximoEstado = "En Balanza Origen";
                    proximoProgreso = 20;
                    break;
                case "En Balanza Origen":
                    proximoEstado = "En Tránsito";
                    proximoProgreso = 60;
                    break;
                case "En Tránsito":
                    proximoEstado = "Descargado";
                    proximoProgreso = 100;
                    break;
                default:
                    proximoEstado = "En Carga";
                    proximoProgreso = 0;
                    break;
            }

            _agroService.ActualizarEstadoViaje(ViajeSeleccionado.Id, proximoEstado, proximoProgreso);
            int idActual = ViajeSeleccionado.Id;
            CargarViajes();
            ViajeSeleccionado = Viajes.FirstOrDefault(v => v.Id == idActual);
        }

        private void EjecutarGuardarNuevoDespacho()
        {
            if (string.IsNullOrWhiteSpace(NuevoChofer) || string.IsNullOrWhiteSpace(NuevaPatente))
                return;

            var viaje = new TransporteViaje
            {
                ChoferNombre = NuevoChofer,
                ChoferDni = "30.812.441",
                PatenteCamion = NuevaPatente,
                PatenteAcoplado = "AA-801-BB",
                EmpresaTransporte = "Expreso Norte SRL",
                OrigenCampo = NuevoOrigen,
                DestinoAcopio = NuevoDestino,
                GranoTransportado = NuevoGrano,
                PesoNetoTn = NuevasToneladas,
                FechaSalida = DateTime.Now,
                FechaLlegadaEstimada = DateTime.Now.AddHours(3),
                Estado = "En Balanza Origen",
                ProgresoRuta = 15,
                Observaciones = "Despacho emitido desde plataforma SiTech con Carta de Porte digital."
            };

            _agroService.AgregarViajeTransporte(viaje);
            CargarViajes();
            ViajeSeleccionado = viaje;
            MostrarDialogoNuevoDespacho = false;
        }
    }
}
