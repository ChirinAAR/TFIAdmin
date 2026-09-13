using System;
using System.Collections.Generic;

namespace SiTech.AgroLogistica.Models
{
    public class EstacionClimatica
    {
        public int Id { get; set; }
        public string NombreEstacion { get; set; } = string.Empty; // e.g. "Estación Tafí Viejo"
        public string Provincia { get; set; } = string.Empty;       // Tucumán, Salta, etc.
        public double TemperaturaC { get; set; }                   // e.g. 24.8 °C
        public double HumedadRelativaPorcentaje { get; set; }      // e.g. 62%
        public double VientoVelocidadKmH { get; set; }             // e.g. 14 km/h
        public string VientoDireccion { get; set; } = "SE";
        public double PrecipitacionUltimas24hMm { get; set; }      // e.g. 18.2 mm
        public double PresionHpa { get; set; }                     // e.g. 1012 hPa
        public string EstadoCielo { get; set; } = "Despejado";     // Despejado, Parcialmente Nublado, Tormenta, Lluvia
        public string IndiceUV { get; set; } = "Moderado";
        public DateTime UltimaLectura { get; set; } = DateTime.Now;
    }

    public class AlertaMeteorologica
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string TipoAlerta { get; set; } = "Clima"; // Helada Tardía, Tormenta Fuerte, Viento Zonda, Plaga/Fitosanitaria, Granizo
        public string NivelSeveridad { get; set; } = "Media"; // Informativa, Media, Crítica / Urgente
        public string ZonaAfectada { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string RecomendacionOperativa { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public bool Activa { get; set; } = true;

        public string ColorSeveridad => NivelSeveridad switch
        {
            "Crítica" => "#DC2626", // Rojo
            "Media" => "#D97706",   // Ámbar
            _ => "#2563EB"          // Azul
        };
    }
}
