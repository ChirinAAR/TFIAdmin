using System;

namespace SiTech.AgroLogistica.Models
{
    public class CultivoLote
    {
        public int Id { get; set; }
        public string CodigoLote { get; set; } = string.Empty; // e.g. "LT-MAN-01"
        public string Nombre { get; set; } = string.Empty;     // e.g. "Lote El Manantial Norte"
        public string FincaEstablecimiento { get; set; } = string.Empty; // e.g. "Finca Santa Rosa - Leales"
        public string ZonaUbicacion { get; set; } = string.Empty; // e.g. "Leales, Tucumán"
        public double Hectareas { get; set; }
        public string TipoCultivo { get; set; } = string.Empty;   // Soja, Maíz, Trigo, Caña de Azúcar, Limón, Poroto
        public string VariedadHibrido { get; set; } = string.Empty; // e.g. "DM 46i20 IPRO"
        public string EstadoFenologico { get; set; } = string.Empty; // e.g. "R3 - Inicio de formación de vainas"
        public double HumedadSueloPorcentaje { get; set; } // e.g. 68.5%
        public double IndiceVerdeNDVI { get; set; }         // e.g. 0.78
        public string EstadoSanitario { get; set; } = "Óptimo"; // Óptimo, Monitoreo Preventivo, Alerta de Plaga, Estrés Hídrico
        public DateTime FechaSiembra { get; set; }
        public DateTime FechaEstimadaCosecha { get; set; }
        public double RindeEsperadoTnHa { get; set; }
        public string NotasObservaciones { get; set; } = string.Empty;

        // Helpers de presentación
        public string ResumenSuperficie => $"{Hectareas:N1} Ha";
        public string ResumenNDVI => $"{IndiceVerdeNDVI:F2}";
        public string ResumenHumedad => $"{HumedadSueloPorcentaje:F1}%";
    }
}
