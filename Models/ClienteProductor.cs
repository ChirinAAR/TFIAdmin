using System;

namespace SiTech.AgroLogistica.Models
{
    public class ClienteProductor
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string TipoCliente { get; set; } = "Productor Agropecuario"; // Productor Agropecuario, Acopiador, Cooperativa Agrícola, Industria Molienda, Exportador
        public string Localidad { get; set; } = string.Empty;
        public string Provincia { get; set; } = "Tucumán";
        public double HectareasOperadas { get; set; }
        public string ContactoNombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activo"; // Activo, Suspendido, Potencial
        public DateTime FechaAlta { get; set; } = DateTime.Now;
        public int ContratosVigentes { get; set; } = 1;
    }
}
