using System;

namespace SiTech.AgroLogistica.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Legajo { get; set; } = string.Empty; // e.g. "LEG-0419"
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string PuestoRol { get; set; } = string.Empty; // Ingeniero Agrónomo de Campo, Chofer de Carga Pesada, Operador de Cosechadora, Analista de Calidad en Silos, Supervisor Logístico
        public string Departamento { get; set; } = "Operaciones"; // Operaciones Agronómicas, Logística y Flota, Calidad y Acopio, Mantenimiento
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EstadoDisponibilidad { get; set; } = "Disponible"; // Disponible, Asignado a Cosecha, En Tránsito, En Descanso
        public string UbicacionActual { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; } = DateTime.Now.AddYears(-2);
    }
}
