using System;

namespace SiTech.AgroLogistica.Models
{
    public class TransporteViaje
    {
        public int Id { get; set; }
        public string CartaPorteNumero { get; set; } = string.Empty; // e.g. "CTG-9048-2849102"
        public string PatenteCamion { get; set; } = string.Empty;    // e.g. "AF-302-XY"
        public string PatenteAcoplado { get; set; } = string.Empty;  // e.g. "AD-991-ZZ"
        public string ChoferNombre { get; set; } = string.Empty;     // e.g. "Marcos Argañaraz"
        public string ChoferDni { get; set; } = string.Empty;
        public string EmpresaTransporte { get; set; } = string.Empty; // e.g. "Expreso Norte SRL"
        public string OrigenCampo { get; set; } = string.Empty;      // e.g. "Lote El Manantial (Leales, Tucumán)"
        public string DestinoAcopio { get; set; } = string.Empty;    // e.g. "Planta de Acopio Central SiTech - Banda Río Salí"
        public string GranoTransportado { get; set; } = string.Empty;// Soja, Maíz, etc.
        public double PesoNetoTn { get; set; }                       // Toneladas netas (ej. 31.4 Tn)
        public DateTime FechaSalida { get; set; }
        public DateTime? FechaLlegadaEstimada { get; set; }
        public string Estado { get; set; } = "En Carga"; // En Carga, En Balanza Origen, En Tránsito, En Balanza Destino, Descargado, Demorado
        public double ProgresoRuta { get; set; } // 0 a 100%
        public string Observaciones { get; set; } = string.Empty;

        public string ResumenCarga => $"{PesoNetoTn:N1} Tn - {GranoTransportado}";
        public string ResumenVehiculo => $"{PatenteCamion} / {PatenteAcoplado}";
    }
}
