using System;

namespace SiTech.AgroLogistica.Models
{
    public class AcopioSilo
    {
        public int Id { get; set; }
        public string Identificador { get; set; } = string.Empty; // e.g. "Silo Vertical #1"
        public string PlantaAcopio { get; set; } = string.Empty;   // e.g. "Planta Este - Banda del Río Salí"
        public string TipoEstructura { get; set; } = string.Empty; // "Silo Chapa Fondo Cónico", "Silo Plano", "Silo Bolsa Silomax"
        public string GranoAlmacenado { get; set; } = string.Empty;// Soja, Maíz, Trigo
        public double CapacidadMaximaTn { get; set; }              // e.g. 5000 Tn
        public double StockActualTn { get; set; }                  // e.g. 3850 Tn
        public double HumedadGranoPorcentaje { get; set; }          // e.g. 13.2%
        public double TemperaturaGranoC { get; set; }              // e.g. 21.5 °C
        public string EstadoAireacion { get; set; } = "Normal";    // Normal, Aireación Activa, Enfriamiento, En Peligro Calentamiento
        public DateTime UltimaInspeccion { get; set; }
        public string CalidadGrano { get; set; } = "Grado 1";      // Grado 1, Grado 2, Fuera de Estándar

        public double PorcentajeOcupacion => CapacidadMaximaTn > 0 ? (StockActualTn / CapacidadMaximaTn) * 100.0 : 0;
        public double CapacidadDisponibleTn => Math.Max(0, CapacidadMaximaTn - StockActualTn);
        public string ResumenStock => $"{StockActualTn:N0} / {CapacidadMaximaTn:N0} Tn ({PorcentajeOcupacion:F1}%)";
    }
}
