using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class ClientesViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private string _textoBusqueda = string.Empty;
        private ClienteProductor? _clienteSeleccionado;
        private bool _mostrarDialogoNuevo;

        // Propiedades de nuevo cliente
        private string _nuevaRazonSocial = string.Empty;
        private string _nuevoCuit = string.Empty;
        private string _nuevoTipo = "Productor Agropecuario";
        private string _nuevaLocalidad = "Leales";
        private string _nuevaProvincia = "Tucumán";
        private double _nuevasHectareas = 500;
        private string _nuevoContacto = string.Empty;
        private string _nuevoTelefono = string.Empty;

        public ClientesViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Clientes = new ObservableCollection<ClienteProductor>();

            AbrirNuevoCommand = new RelayCommand(() => MostrarDialogoNuevo = true);
            CancelarNuevoCommand = new RelayCommand(() => MostrarDialogoNuevo = false);
            GuardarNuevoCommand = new RelayCommand(EjecutarGuardarNuevo);

            CargarClientes();
        }

        public ObservableCollection<ClienteProductor> Clientes { get; }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                    CargarClientes();
            }
        }

        public ClienteProductor? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set => SetProperty(ref _clienteSeleccionado, value);
        }

        public bool MostrarDialogoNuevo
        {
            get => _mostrarDialogoNuevo;
            set => SetProperty(ref _mostrarDialogoNuevo, value);
        }

        public string NuevaRazonSocial { get => _nuevaRazonSocial; set => SetProperty(ref _nuevaRazonSocial, value); }
        public string NuevoCuit { get => _nuevoCuit; set => SetProperty(ref _nuevoCuit, value); }
        public string NuevoTipo { get => _nuevoTipo; set => SetProperty(ref _nuevoTipo, value); }
        public string NuevaLocalidad { get => _nuevaLocalidad; set => SetProperty(ref _nuevaLocalidad, value); }
        public string NuevaProvincia { get => _nuevaProvincia; set => SetProperty(ref _nuevaProvincia, value); }
        public double NuevasHectareas { get => _nuevasHectareas; set => SetProperty(ref _nuevasHectareas, value); }
        public string NuevoContacto { get => _nuevoContacto; set => SetProperty(ref _nuevoContacto, value); }
        public string NuevoTelefono { get => _nuevoTelefono; set => SetProperty(ref _nuevoTelefono, value); }

        public ICommand AbrirNuevoCommand { get; }
        public ICommand CancelarNuevoCommand { get; }
        public ICommand GuardarNuevoCommand { get; }

        public void CargarClientes()
        {
            var consulta = _agroService.ObtenerClientes().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                consulta = consulta.Where(c =>
                    c.RazonSocial.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    c.Cuit.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    c.Localidad.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));
            }

            Clientes.Clear();
            foreach (var item in consulta)
            {
                Clientes.Add(item);
            }

            if (ClienteSeleccionado == null && Clientes.Any())
                ClienteSeleccionado = Clientes.First();
        }

        private void EjecutarGuardarNuevo()
        {
            if (string.IsNullOrWhiteSpace(NuevaRazonSocial)) return;

            var nuevo = new ClienteProductor
            {
                RazonSocial = NuevaRazonSocial,
                Cuit = string.IsNullOrWhiteSpace(NuevoCuit) ? "30-75123987-9" : NuevoCuit,
                TipoCliente = NuevoTipo,
                Localidad = NuevaLocalidad,
                Provincia = NuevaProvincia,
                HectareasOperadas = NuevasHectareas,
                ContactoNombre = NuevoContacto,
                Telefono = NuevoTelefono,
                Email = "contacto@" + NuevaRazonSocial.Replace(" ", "").ToLower() + ".com.ar",
                Estado = "Activo",
                ContratosVigentes = 1
            };

            _agroService.AgregarCliente(nuevo);
            CargarClientes();
            ClienteSeleccionado = nuevo;
            MostrarDialogoNuevo = false;

            NuevaRazonSocial = string.Empty;
            NuevoContacto = string.Empty;
        }
    }
}
