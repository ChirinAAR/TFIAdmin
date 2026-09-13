using System.Collections.Generic;
using SiTech.AgroLogistica.Models;

namespace SiTech.AgroLogistica.Services.Interfaces
{
    public interface IAgroDataService
    {
        // Cultivos y Lotes
        IReadOnlyList<CultivoLote> ObtenerLotes();
        void AgregarLote(CultivoLote lote);
        CultivoLote? ObtenerLotePorCodigo(string codigo);

        // Cosecha
        IReadOnlyList<CosechaLabor> ObtenerLaboresCosecha();
        void AgregarLaborCosecha(CosechaLabor labor);
        void RegistrarAvanceCosecha(int idLabor, double toneladasAdicionales, double nuevoAvance);

        // Transporte
        IReadOnlyList<TransporteViaje> ObtenerViajesTransporte();
        void AgregarViajeTransporte(TransporteViaje viaje);
        void ActualizarEstadoViaje(int idViaje, string nuevoEstado, double nuevoProgreso);

        // Acopio y Silos
        IReadOnlyList<AcopioSilo> ObtenerSilos();
        void ActualizarStockSilo(int idSilo, double deltaToneladas);

        // Clima y Alertas
        IReadOnlyList<EstacionClimatica> ObtenerEstacionesClima();
        IReadOnlyList<AlertaMeteorologica> ObtenerAlertas();
        void AgregarAlerta(AlertaMeteorologica alerta);
        void ResolverAlerta(int idAlerta);

        // Trazabilidad
        IReadOnlyList<TrazabilidadItem> ObtenerTrazabilidades();
        TrazabilidadItem? BuscarTrazabilidad(string codigoOTexto);

        // Clientes Productores
        IReadOnlyList<ClienteProductor> ObtenerClientes();
        void AgregarCliente(ClienteProductor cliente);

        // Campañas y Proyectos
        IReadOnlyList<CampanaProyecto> ObtenerCampanas();
        void AgregarCampana(CampanaProyecto campana);

        // Empleados y Operarios
        IReadOnlyList<Empleado> ObtenerEmpleados();
        void AgregarEmpleado(Empleado empleado);

        // Dashboard & KPIs
        List<IndicadorKpi> ObtenerKpisPrincipales();
    }
}
