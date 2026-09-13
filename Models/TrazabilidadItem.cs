using System;
using System.Collections.Generic;

namespace SiTech.AgroLogistica.Models
{
    public class HitoTrazabilidad
    {
        public int Paso { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string ResponsableOEntidad { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public bool Completado { get; set; } = true;
        public string DocumentoAsociado { get; set; } = string.Empty;
    }

    public class TrazabilidadItem
    {
        public int Id { get; set; }
        public string CodigoTrazabilidad { get; set; } = string.Empty; // e.g. "TRZ-2026-SOJ-0914"
        public string NombrePartida { get; set; } = string.Empty;     // e.g. "Lote Soja Primera - Campaña 2026"
        public string Cultivo { get; set; } = string.Empty;
        public string LoteOrigen { get; set; } = string.Empty;
        public string Establecimiento { get; set; } = string.Empty;
        public string ProductorCliente { get; set; } = string.Empty;
        public double ToneladasTotales { get; set; }
        public string CalidadCertificada { get; set; } = "Grado 1 - Libre OGM";
        public string SiloUbicacionActual { get; set; } = string.Empty;
        public string DestinoFinalPrevisto { get; set; } = string.Empty;
        public string EstadoCadena { get; set; } = "En Acopio"; // En Campo, En Tránsito, En Acopio, En Puerto, Certificado Exportación
        public DateTime FechaCreacion { get; set; }
        public List<HitoTrazabilidad> HistorialHitos { get; set; } = new();

        public string ResumenGeneral => $"{CodigoTrazabilidad} | {Cultivo} ({ToneladasTotales:N1} Tn) - {EstadoCadena}";
    }
}
