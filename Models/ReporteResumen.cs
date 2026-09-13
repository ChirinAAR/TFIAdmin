using System;

namespace SiTech.AgroLogistica.Models
{
    public class FilaReporte
    {
        public string Columna1 { get; set; } = string.Empty;
        public string Columna2 { get; set; } = string.Empty;
        public string Columna3 { get; set; } = string.Empty;
        public string Columna4 { get; set; } = string.Empty;
        public string Columna5 { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class IndicadorKpi
    {
        public string Titulo { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
        public string Tendencia { get; set; } = string.Empty; // "+5%", "-2%", etc.
        public string IconoKey { get; set; } = string.Empty;
        public string ColorFondo { get; set; } = "#2E7D32";
    }
}
