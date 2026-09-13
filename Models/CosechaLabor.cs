using System;

namespace SiTech.AgroLogistica.Models
{
    public class CosechaLabor
    {
        public int Id { get; set; }
        public string CodigoLabor { get; set; } = string.Empty; // e.g. "COS-2026-081"
        public string LoteNombre { get; set; } = string.Empty;
        public string TipoCultivo { get; set; } = string.Empty;
        public double HectareasLabor { get; set; }
        public DateTime FechaPlanificada { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public double ToneladasEstimadas { get; set; }
        public double ToneladasRecolectadas { get; set; }
        public double RindeRealTnHa => HectareasLabor > 0 ? (ToneladasRecolectadas / HectareasLabor) : 0;
        public string MaquinariaAsignada { get; set; } = string.Empty; // e.g. "Cosechadora John Deere S780 #2 + Tolva Cestari 35k"
        public string CuadrillaResponsable { get; set; } = string.Empty; // e.g. "Equipo Cosecha Alfa - Ing. Peralta"
        public string Estado { get; set; } = "Planificada"; // Planificada, En Curso, Finalizada, Demorada por Clima
        public double PorcentajeAvance { get; set; } // 0 a 100%
        public string SiloDestinoSugerido { get; set; } = string.Empty;

        public string ResumenTon => $"{ToneladasRecolectadas:N1} / {ToneladasEstimadas:N1} Tn";
        public string ResumenRinde => $"{RindeRealTnHa:F2} Tn/Ha";
    }
}
