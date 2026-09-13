using System;

namespace SiTech.AgroLogistica.Models
{
    public class CampanaProyecto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty; // e.g. "CAMP-2025/26-GRUESA"
        public string Nombre { get; set; } = string.Empty; // e.g. "Campaña Gruesa 2025/2026 - Soja y Maíz NOA"
        public string TipoCiclo { get; set; } = "Gruesa"; // Gruesa, Fina, Zafra Azucarera, Temporada Cítrica
        public DateTime FechaInicio { get; set; }
        public DateTime FechaEstimadaCierre { get; set; }
        public string ResponsableGeneral { get; set; } = string.Empty;
        public double HectareasPlanificadas { get; set; }
        public double ToneladasObjetivo { get; set; }
        public double PresupuestoEstimadoUsd { get; set; }
        public double AvancePorcentaje { get; set; } // 0 a 100%
        public string Estado { get; set; } = "En Curso"; // Planificada, En Curso, Finalizada, En Liquidación

        public string ResumenAvance => $"{AvancePorcentaje:F0}%";
        public string ResumenHectareas => $"{HectareasPlanificadas:N0} Ha";
    }
}
