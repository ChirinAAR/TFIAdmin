using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class EmpleadosViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private string _textoBusqueda = string.Empty;
        private Empleado? _empleadoSeleccionado;
        private bool _mostrarDialogoNuevo;

        // Propiedades nuevo empleado
        private string _nuevoNombre = string.Empty;
        private string _nuevoDni = string.Empty;
        private string _nuevoPuesto = "Ingeniero Agrónomo de Campo";
        private string _nuevoDepartamento = "Operaciones Agronómicas";
        private string _nuevoTelefono = string.Empty;
        private string _nuevaUbicacion = "Base Central Leales";

        public EmpleadosViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Empleados = new ObservableCollection<Empleado>();

            AbrirNuevoCommand = new RelayCommand(() => MostrarDialogoNuevo = true);
            CancelarNuevoCommand = new RelayCommand(() => MostrarDialogoNuevo = false);
            GuardarNuevoCommand = new RelayCommand(EjecutarGuardarNuevo);

            CargarEmpleados();
        }

        public ObservableCollection<Empleado> Empleados { get; }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                    CargarEmpleados();
            }
        }

        public Empleado? EmpleadoSeleccionado
        {
            get => _empleadoSeleccionado;
            set => SetProperty(ref _empleadoSeleccionado, value);
        }

        public bool MostrarDialogoNuevo
        {
            get => _mostrarDialogoNuevo;
            set => SetProperty(ref _mostrarDialogoNuevo, value);
        }

        public string NuevoNombre { get => _nuevoNombre; set => SetProperty(ref _nuevoNombre, value); }
        public string NuevoDni { get => _nuevoDni; set => SetProperty(ref _nuevoDni, value); }
        public string NuevoPuesto { get => _nuevoPuesto; set => SetProperty(ref _nuevoPuesto, value); }
        public string NuevoDepartamento { get => _nuevoDepartamento; set => SetProperty(ref _nuevoDepartamento, value); }
        public string NuevoTelefono { get => _nuevoTelefono; set => SetProperty(ref _nuevoTelefono, value); }
        public string NuevaUbicacion { get => _nuevaUbicacion; set => SetProperty(ref _nuevaUbicacion, value); }

        public ICommand AbrirNuevoCommand { get; }
        public ICommand CancelarNuevoCommand { get; }
        public ICommand GuardarNuevoCommand { get; }

        public void CargarEmpleados()
        {
            var consulta = _agroService.ObtenerEmpleados().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                consulta = consulta.Where(e =>
                    e.NombreCompleto.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    e.Legajo.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                    e.PuestoRol.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));
            }

            Empleados.Clear();
            foreach (var item in consulta)
            {
                Empleados.Add(item);
            }

            if (EmpleadoSeleccionado == null && Empleados.Any())
                EmpleadoSeleccionado = Empleados.First();
        }

        private void EjecutarGuardarNuevo()
        {
            if (string.IsNullOrWhiteSpace(NuevoNombre)) return;

            var nuevo = new Empleado
            {
                Legajo = $"LEG-{DateTime.Now.Millisecond:D4}",
                NombreCompleto = NuevoNombre,
                Dni = string.IsNullOrWhiteSpace(NuevoDni) ? "34.120.999" : NuevoDni,
                PuestoRol = NuevoPuesto,
                Departamento = NuevoDepartamento,
                Telefono = NuevoTelefono,
                Email = NuevoNombre.ToLower().Replace(" ", ".") + "@sitech.com.ar",
                EstadoDisponibilidad = "Disponible",
                UbicacionActual = NuevaUbicacion,
                FechaIngreso = DateTime.Now
            };

            _agroService.AgregarEmpleado(nuevo);
            CargarEmpleados();
            EmpleadoSeleccionado = nuevo;
            MostrarDialogoNuevo = false;

            NuevoNombre = string.Empty;
        }
    }
}
