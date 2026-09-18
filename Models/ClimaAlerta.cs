using System;
using System.Collections.Generic;

namespace SiTech.AgroLogistica.Models
{
    public class PronosticoDia
    {
        public string DiaSemana { get; set; } = string.Empty;
        public string FechaCorta { get; set; } = string.Empty;
        public double TempMinC { get; set; }
        public double TempMaxC { get; set; }
        public string Condicion { get; set; } = "Soleado";
        public string Icono { get; set; } = "☀️";
        public int ProbabilidadLluviaPorcentaje { get; set; }
        public double VientoMaxKmH { get; set; }
        public string EstadoVentana { get; set; } = "Óptima";
    }

    public class EstacionClimatica
    {
        public int Id { get; set; }
        public string NombreEstacion { get; set; } = string.Empty; // e.g. "Estación Tafí Viejo"
        public string Provincia { get; set; } = string.Empty;       // Tucumán, Salta, etc.
        public double TemperaturaC { get; set; }                   // e.g. 24.8 °C
        public double SensacionTermicaC { get; set; } = 25.0;
        public double HumedadRelativaPorcentaje { get; set; }      // e.g. 62%
        public double PuntoRocioC { get; set; } = 15.2;
        public double VientoVelocidadKmH { get; set; }             // e.g. 14 km/h
        public double RafagasKmH { get; set; } = 22.0;
        public string VientoDireccion { get; set; } = "SE";
        public double PrecipitacionUltimas24hMm { get; set; }      // e.g. 18.2 mm
        public double LluviaAcumuladaMesMm { get; set; } = 64.0;
        public double EvapotranspiracionEtoMm { get; set; } = 4.2;
        public double PresionHpa { get; set; }                     // e.g. 1012 hPa
        public string EstadoCielo { get; set; } = "Despejado";     // Despejado, Parcialmente Nublado, Tormenta, Lluvia
        public string IndiceUV { get; set; } = "Moderado";
        public double RadiacionSolarWm2 { get; set; } = 740.0;
        public double DeltaTC { get; set; } = 4.8;
        public string CondicionPulverizacion { get; set; } = "Ventana Óptima";
        public string DiagnosticoVentana { get; set; } = "Condiciones ideales para pulverización terrestre. Viento calmo con baja deriva y rango térmico propicio.";
        public List<PronosticoDia> PronosticoExtendido { get; set; } = new();
        public DateTime UltimaLectura { get; set; } = DateTime.Now;
    }

    public class AlertaMeteorologica
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string ModuloOrigen { get; set; } = "Clima"; // Cultivos, Cosecha, Transporte, Acopio, Clima, Trazabilidad
        public string TipoAlerta { get; set; } = "Clima"; // Helada Tardía, Tormenta Fuerte, Viento Zonda, Plaga/Fitosanitaria, Granizo, Mecánica, Temperatura, etc.
        public string NivelSeveridad { get; set; } = "Media"; // Informativa, Media, Alta, Crítica
        public string ZonaAfectada { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string RecomendacionOperativa { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public bool Activa { get; set; } = true;

        public string IconoModulo => ModuloOrigen switch
        {
            "Cultivos" => "🌱",
            "Cosecha" => "🚜",
            "Transporte" => "🚚",
            "Acopio" => "🏭",
            "Clima" => "🌤️",
            "Trazabilidad" => "🔍",
            _ => "⚠️"
        };

        public string ColorSeveridad => NivelSeveridad switch
        {
            "Crítica" => "#DC2626", // Rojo
            "Alta" => "#EA580C",    // Naranja rojizo
            "Media" => "#D97706",   // Ámbar
            _ => "#2563EB"          // Azul
        };

        public string ColorSeveridadFondo => NivelSeveridad switch
        {
            "Crítica" => "#FEF2F2",
            "Alta" => "#FFF7ED",
            "Media" => "#FFFBEB",
            _ => "#EFF6FF"
        };

        public string ColorSeveridadBorde => NivelSeveridad switch
        {
            "Crítica" => "#F87171",
            "Alta" => "#FB923C",
            "Media" => "#FBBF24",
            _ => "#60A5FA"
        };

        public string ColorBadgeModulo => ModuloOrigen switch
        {
            "Cultivos" => "#166534",
            "Cosecha" => "#92400E",
            "Transporte" => "#0369A1",
            "Acopio" => "#B45309",
            "Clima" => "#4338CA",
            "Trazabilidad" => "#0F766E",
            _ => "#374151"
        };

        public string FondoBadgeModulo => ModuloOrigen switch
        {
            "Cultivos" => "#DCFCE7",
            "Cosecha" => "#FEF3C7",
            "Transporte" => "#E0F2FE",
            "Acopio" => "#FEF3C7",
            "Clima" => "#EEF2FF",
            "Trazabilidad" => "#CCFBF1",
            _ => "#F3F4F6"
        };

        public string TiempoTranscurrido
        {
            get
            {
                var diff = DateTime.Now - FechaEmision;
                if (diff.TotalMinutes < 60)
                    return $"Hace {Math.Max(1, (int)diff.TotalMinutes)} min";
                if (diff.TotalHours < 24)
                    return $"Hace {(int)diff.TotalHours} h";
                return $"Hace {(int)diff.TotalDays} d";
            }
        }
    }
}
