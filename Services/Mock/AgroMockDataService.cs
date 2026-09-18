using System;
using System.Collections.Generic;
using System.Linq;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;

namespace SiTech.AgroLogistica.Services.Mock
{
    public class AgroMockDataService : IAgroDataService
    {
        private readonly List<CultivoLote> _lotes;
        private readonly List<CosechaLabor> _labores;
        private readonly List<TransporteViaje> _viajes;
        private readonly List<AcopioSilo> _silos;
        private readonly List<EstacionClimatica> _estaciones;
        private readonly List<AlertaMeteorologica> _alertas;
        private readonly List<TrazabilidadItem> _trazabilidades;
        private readonly List<ClienteProductor> _clientes;
        private readonly List<CampanaProyecto> _campanas;
        private readonly List<Empleado> _empleados;

        public AgroMockDataService()
        {
            _lotes = CargarLotesIniciales();
            _labores = CargarLaboresIniciales();
            _viajes = CargarViajesIniciales();
            _silos = CargarSilosIniciales();
            _estaciones = CargarEstacionesIniciales();
            _alertas = CargarAlertasIniciales();
            _trazabilidades = CargarTrazabilidadesIniciales();
            _clientes = CargarClientesIniciales();
            _campanas = CargarCampanasIniciales();
            _empleados = CargarEmpleadosIniciales();
        }

        #region Cultivos y Lotes
        public IReadOnlyList<CultivoLote> ObtenerLotes() => _lotes.AsReadOnly();

        public void AgregarLote(CultivoLote lote)
        {
            lote.Id = _lotes.Any() ? _lotes.Max(l => l.Id) + 1 : 1;
            if (string.IsNullOrWhiteSpace(lote.CodigoLote))
                lote.CodigoLote = $"LT-NOA-{lote.Id:D3}";
            _lotes.Insert(0, lote);
        }

        public CultivoLote? ObtenerLotePorCodigo(string codigo) =>
            _lotes.FirstOrDefault(l => l.CodigoLote.Equals(codigo, StringComparison.OrdinalIgnoreCase) ||
                                       l.Nombre.Contains(codigo, StringComparison.OrdinalIgnoreCase));
        #endregion

        #region Cosecha
        public IReadOnlyList<CosechaLabor> ObtenerLaboresCosecha() => _labores.AsReadOnly();

        public void AgregarLaborCosecha(CosechaLabor labor)
        {
            labor.Id = _labores.Any() ? _labores.Max(l => l.Id) + 1 : 1;
            if (string.IsNullOrWhiteSpace(labor.CodigoLabor))
                labor.CodigoLabor = $"COS-2026-{labor.Id:D3}";
            _labores.Insert(0, labor);
        }

        public void RegistrarAvanceCosecha(int idLabor, double toneladasAdicionales, double nuevoAvance)
        {
            var labor = _labores.FirstOrDefault(l => l.Id == idLabor);
            if (labor != null)
            {
                labor.ToneladasRecolectadas += toneladasAdicionales;
                labor.PorcentajeAvance = Math.Clamp(nuevoAvance, 0, 100);
                if (labor.PorcentajeAvance >= 100)
                {
                    labor.Estado = "Finalizada";
                    labor.FechaFin = DateTime.Now;
                }
                else if (labor.PorcentajeAvance > 0 && labor.Estado == "Planificada")
                {
                    labor.Estado = "En Curso";
                    labor.FechaInicio ??= DateTime.Now;
                }
            }
        }
        #endregion

        #region Transporte
        public IReadOnlyList<TransporteViaje> ObtenerViajesTransporte() => _viajes.AsReadOnly();

        public void AgregarViajeTransporte(TransporteViaje viaje)
        {
            viaje.Id = _viajes.Any() ? _viajes.Max(v => v.Id) + 1 : 1;
            if (string.IsNullOrWhiteSpace(viaje.CartaPorteNumero))
                viaje.CartaPorteNumero = $"CTG-9048-{1000000 + viaje.Id}";
            _viajes.Insert(0, viaje);
        }

        public void ActualizarEstadoViaje(int idViaje, string nuevoEstado, double nuevoProgreso)
        {
            var viaje = _viajes.FirstOrDefault(v => v.Id == idViaje);
            if (viaje != null)
            {
                viaje.Estado = nuevoEstado;
                viaje.ProgresoRuta = Math.Clamp(nuevoProgreso, 0, 100);
            }
        }
        #endregion

        #region Acopio y Silos
        public IReadOnlyList<AcopioSilo> ObtenerSilos() => _silos.AsReadOnly();

        public void ActualizarStockSilo(int idSilo, double deltaToneladas)
        {
            var silo = _silos.FirstOrDefault(s => s.Id == idSilo);
            if (silo != null)
            {
                silo.StockActualTn = Math.Clamp(silo.StockActualTn + deltaToneladas, 0, silo.CapacidadMaximaTn);
                silo.UltimaInspeccion = DateTime.Now;
            }
        }
        #endregion

        #region Clima y Alertas
        public IReadOnlyList<EstacionClimatica> ObtenerEstacionesClima() => _estaciones.AsReadOnly();
        public IReadOnlyList<AlertaMeteorologica> ObtenerAlertas() => _alertas.AsReadOnly();

        public void AgregarAlerta(AlertaMeteorologica alerta)
        {
            alerta.Id = _alertas.Any() ? _alertas.Max(a => a.Id) + 1 : 1;
            alerta.FechaEmision = DateTime.Now;
            alerta.Activa = true;
            _alertas.Insert(0, alerta);
        }

        public void ResolverAlerta(int idAlerta)
        {
            var alerta = _alertas.FirstOrDefault(a => a.Id == idAlerta);
            if (alerta != null)
            {
                alerta.Activa = false;
            }
        }
        #endregion

        #region Trazabilidad
        public IReadOnlyList<TrazabilidadItem> ObtenerTrazabilidades() => _trazabilidades.AsReadOnly();

        public TrazabilidadItem? BuscarTrazabilidad(string codigoOTexto)
        {
            if (string.IsNullOrWhiteSpace(codigoOTexto))
                return _trazabilidades.FirstOrDefault();

            return _trazabilidades.FirstOrDefault(t =>
                t.CodigoTrazabilidad.Contains(codigoOTexto, StringComparison.OrdinalIgnoreCase) ||
                t.LoteOrigen.Contains(codigoOTexto, StringComparison.OrdinalIgnoreCase) ||
                t.ProductorCliente.Contains(codigoOTexto, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region Clientes
        public IReadOnlyList<ClienteProductor> ObtenerClientes() => _clientes.AsReadOnly();

        public void AgregarCliente(ClienteProductor cliente)
        {
            cliente.Id = _clientes.Any() ? _clientes.Max(c => c.Id) + 1 : 1;
            _clientes.Insert(0, cliente);
        }
        #endregion

        #region Campañas
        public IReadOnlyList<CampanaProyecto> ObtenerCampanas() => _campanas.AsReadOnly();

        public void AgregarCampana(CampanaProyecto campana)
        {
            campana.Id = _campanas.Any() ? _campanas.Max(c => c.Id) + 1 : 1;
            _campanas.Insert(0, campana);
        }
        #endregion

        #region Empleados
        public IReadOnlyList<Empleado> ObtenerEmpleados() => _empleados.AsReadOnly();

        public void AgregarEmpleado(Empleado empleado)
        {
            empleado.Id = _empleados.Any() ? _empleados.Max(e => e.Id) + 1 : 1;
            _empleados.Insert(0, empleado);
        }
        #endregion

        #region Dashboard & KPIs
        public List<IndicadorKpi> ObtenerKpisPrincipales()
        {
            double totalHa = _lotes.Sum(l => l.Hectareas);
            double totalTnCosechadas = _labores.Sum(l => l.ToneladasRecolectadas);
            int viajesEnTransito = _viajes.Count(v => v.Estado == "En Tránsito" || v.Estado == "En Carga");
            double totalCapSilos = _silos.Sum(s => s.CapacidadMaximaTn);
            double stockTotal = _silos.Sum(s => s.StockActualTn);
            double ocupacionSilos = totalCapSilos > 0 ? (stockTotal / totalCapSilos) * 100.0 : 0;
            int alertasActivas = _alertas.Count(a => a.Activa);
            int alertasCriticas = _alertas.Count(a => a.Activa && (a.NivelSeveridad == "Crítica" || a.NivelSeveridad == "Alta"));

            return new List<IndicadorKpi>
            {
                new IndicadorKpi
                {
                    Titulo = "SUPERFICIE BAJO MONITOREO",
                    Valor = $"{totalHa:N0} Ha",
                    Subtitulo = $"{_lotes.Count} lotes georreferenciados en NOA",
                    Tendencia = "+12% vs ciclo anterior",
                    IconoKey = "IconSprout",
                    ColorFondo = "#1B5E20"
                },
                new IndicadorKpi
                {
                    Titulo = "PRODUCCIÓN COSECHADA",
                    Valor = $"{totalTnCosechadas:N0} Tn",
                    Subtitulo = "Campaña Gruesa 2025/2026 activa",
                    Tendencia = "Rinde prom: 3.25 Tn/Ha",
                    IconoKey = "IconHarvest",
                    ColorFondo = "#2E7D32"
                },
                new IndicadorKpi
                {
                    Titulo = "LOGÍSTICA EN TRÁNSITO",
                    Valor = $"{viajesEnTransito} Camiones",
                    Subtitulo = $"{_viajes.Where(v => v.Estado == "En Tránsito").Sum(v => v.PesoNetoTn):N0} Tn en ruta hacia acopios",
                    Tendencia = "100% CTG Electrónicas activas",
                    IconoKey = "IconTruck",
                    ColorFondo = "#0284C7"
                },
                new IndicadorKpi
                {
                    Titulo = "CAPACIDAD EN SILOS & ACOPIO",
                    Valor = $"{ocupacionSilos:F1}%",
                    Subtitulo = $"{stockTotal:N0} Tn almacenadas de {totalCapSilos:N0} Tn",
                    Tendencia = "Humedad promedio segura: 13.1%",
                    IconoKey = "IconWarehouse",
                    ColorFondo = "#D97706"
                },
                new IndicadorKpi
                {
                    Titulo = "ALERTAS ACTIVAS DEL SISTEMA",
                    Valor = $"{alertasActivas} Operativas",
                    Subtitulo = "Cultivos, Acopio, Flota y Clima",
                    Tendencia = alertasCriticas > 0 ? $"{alertasCriticas} de alta severidad prioritarias" : "Parámetros dentro de norma",
                    IconoKey = "IconAlert",
                    ColorFondo = alertasCriticas > 0 ? "#DC2626" : "#059669"
                }
            };
        }
        #endregion

        #region Inicialización de Datos NOA
        private static List<CultivoLote> CargarLotesIniciales()
        {
            return new List<CultivoLote>
            {
                new CultivoLote
                {
                    Id = 1,
                    CodigoLote = "LT-LEAL-01",
                    Nombre = "Lote El Manantial Norte",
                    FincaEstablecimiento = "Finca Santa Rosa",
                    ZonaUbicacion = "Leales, Tucumán",
                    Hectareas = 240,
                    TipoCultivo = "Soja",
                    VariedadHibrido = "DM 46i20 IPRO",
                    EstadoFenologico = "R6 - Semilla completa en vaina",
                    HumedadSueloPorcentaje = 68.5,
                    IndiceVerdeNDVI = 0.81,
                    EstadoSanitario = "Óptimo",
                    FechaSiembra = DateTime.Now.AddMonths(-4),
                    FechaEstimadaCosecha = DateTime.Now.AddDays(5),
                    RindeEsperadoTnHa = 3.40,
                    NotasObservaciones = "Cultivo con excelente uniformidad. Cosecha programada para este fin de semana."
                },
                new CultivoLote
                {
                    Id = 2,
                    CodigoLote = "LT-ANTA-04",
                    Nombre = "Lote Cañon de Anta #4",
                    FincaEstablecimiento = "Establecimiento Don Juan",
                    ZonaUbicacion = "Joaquín V. González, Salta",
                    Hectareas = 520,
                    TipoCultivo = "Soja",
                    VariedadHibrido = "Don Mario 50i22 STS",
                    EstadoFenologico = "R8 - Madurez de cosecha",
                    HumedadSueloPorcentaje = 52.0,
                    IndiceVerdeNDVI = 0.62,
                    EstadoSanitario = "Óptimo",
                    FechaSiembra = DateTime.Now.AddMonths(-5),
                    FechaEstimadaCosecha = DateTime.Now.AddDays(-2),
                    RindeEsperadoTnHa = 3.20,
                    NotasObservaciones = "Cosecha finalizada al 100%. Tolvas completando descarga hacia camiones."
                },
                new CultivoLote
                {
                    Id = 3,
                    CodigoLote = "LT-CRUZ-02",
                    Nombre = "Finca San Javier - Parcela 2",
                    FincaEstablecimiento = "Finca San Javier",
                    ZonaUbicacion = "Cruz Alta, Tucumán",
                    Hectareas = 180,
                    TipoCultivo = "Maíz",
                    VariedadHibrido = "Dekalb 72-27 VT3P",
                    EstadoFenologico = "R4 - Grano pastoso",
                    HumedadSueloPorcentaje = 71.0,
                    IndiceVerdeNDVI = 0.84,
                    EstadoSanitario = "Monitoreo Preventivo",
                    FechaSiembra = DateTime.Now.AddMonths(-3),
                    FechaEstimadaCosecha = DateTime.Now.AddDays(25),
                    RindeEsperadoTnHa = 8.50,
                    NotasObservaciones = "Se detectó presencia focalizada de oruga barrenadora. Tratamiento foliar aplicado."
                },
                new CultivoLote
                {
                    Id = 4,
                    CodigoLote = "LT-TAFI-07",
                    Nombre = "Los Naranjales - Cuadro A",
                    FincaEstablecimiento = "Citrícola Tafí Grande",
                    ZonaUbicacion = "Tafí Viejo, Tucumán",
                    Hectareas = 95,
                    TipoCultivo = "Limón",
                    VariedadHibrido = "Eureka Clon 22 (Pie Citrange)",
                    EstadoFenologico = "Fructificación / Engrose de frutos",
                    HumedadSueloPorcentaje = 76.5,
                    IndiceVerdeNDVI = 0.88,
                    EstadoSanitario = "Óptimo",
                    FechaSiembra = DateTime.Now.AddYears(-6),
                    FechaEstimadaCosecha = DateTime.Now.AddDays(15),
                    RindeEsperadoTnHa = 24.0,
                    NotasObservaciones = "Calibre homogéneo para exportación a la Unión Europea. Riego por goteo operativo."
                },
                new CultivoLote
                {
                    Id = 5,
                    CodigoLote = "LT-MONT-03",
                    Nombre = "Parcela Santa Lucía",
                    FincaEstablecimiento = "Colonia Azucarera del Sur",
                    ZonaUbicacion = "Monteros, Tucumán",
                    Hectareas = 420,
                    TipoCultivo = "Caña de Azúcar",
                    VariedadHibrido = "LCP 85-384",
                    EstadoFenologico = "Gran período de crecimiento",
                    HumedadSueloPorcentaje = 69.0,
                    IndiceVerdeNDVI = 0.85,
                    EstadoSanitario = "Óptimo",
                    FechaSiembra = DateTime.Now.AddYears(-1),
                    FechaEstimadaCosecha = DateTime.Now.AddMonths(2),
                    RindeEsperadoTnHa = 75.0,
                    NotasObservaciones = "Zafra estimada para junio. Buen contenido de sacarosa y maduración."
                },
                new CultivoLote
                {
                    Id = 6,
                    CodigoLote = "LT-ROSA-01",
                    Nombre = "Frontera Este - Lote 1",
                    FincaEstablecimiento = "Agropecuaria El Quebracho",
                    ZonaUbicacion = "Rosario de la Frontera, Salta",
                    Hectareas = 310,
                    TipoCultivo = "Poroto",
                    VariedadHibrido = "Poroto Negro Camatí",
                    EstadoFenologico = "R5 - Llenado de semillas",
                    HumedadSueloPorcentaje = 59.0,
                    IndiceVerdeNDVI = 0.74,
                    EstadoSanitario = "Óptimo",
                    FechaSiembra = DateTime.Now.AddMonths(-2),
                    FechaEstimadaCosecha = DateTime.Now.AddDays(35),
                    RindeEsperadoTnHa = 1.65,
                    NotasObservaciones = "Buena floración y cuajado. Cero plagas de mosca blanca reportadas."
                }
            };
        }

        private static List<CosechaLabor> CargarLaboresIniciales()
        {
            return new List<CosechaLabor>
            {
                new CosechaLabor
                {
                    Id = 1,
                    CodigoLabor = "COS-2026-041",
                    LoteNombre = "Lote El Manantial Norte",
                    TipoCultivo = "Soja",
                    HectareasLabor = 240,
                    FechaPlanificada = DateTime.Now.AddDays(-2),
                    FechaInicio = DateTime.Now.AddDays(-2),
                    ToneladasEstimadas = 816,
                    ToneladasRecolectadas = 540,
                    MaquinariaAsignada = "JD S780 #1 + JD S770 #2 + Tolva Cestari 35k",
                    CuadrillaResponsable = "Equipo Cosecha Alfa (Ing. Paz)",
                    Estado = "En Curso",
                    PorcentajeAvance = 68,
                    SiloDestinoSugerido = "Silo Vertical #1 (Banda Río Salí)"
                },
                new CosechaLabor
                {
                    Id = 2,
                    CodigoLabor = "COS-2026-039",
                    LoteNombre = "Lote Cañon de Anta #4",
                    TipoCultivo = "Soja",
                    HectareasLabor = 520,
                    FechaPlanificada = DateTime.Now.AddDays(-10),
                    FechaInicio = DateTime.Now.AddDays(-9),
                    FechaFin = DateTime.Now.AddDays(-1),
                    ToneladasEstimadas = 1664,
                    ToneladasRecolectadas = 1680,
                    MaquinariaAsignada = "Flota Case IH 8250 (3 unidades) + Tolvas Akron",
                    CuadrillaResponsable = "Cuadrilla Contratista Salta",
                    Estado = "Finalizada",
                    PorcentajeAvance = 100,
                    SiloDestinoSugerido = "Silos Puerto Rosario / Acopio Joaquín V. González"
                },
                new CosechaLabor
                {
                    Id = 3,
                    CodigoLabor = "COS-2026-042",
                    LoteNombre = "Finca San Javier - Parcela 2",
                    TipoCultivo = "Maíz",
                    HectareasLabor = 180,
                    FechaPlanificada = DateTime.Now.AddDays(20),
                    ToneladasEstimadas = 1530,
                    ToneladasRecolectadas = 0,
                    MaquinariaAsignada = "Cosechadora New Holland CR 8.90 con cabezal maicero",
                    CuadrillaResponsable = "Equipo Cosecha Beta",
                    Estado = "Planificada",
                    PorcentajeAvance = 0,
                    SiloDestinoSugerido = "Silo Vertical #2 (Maíz)"
                },
                new CosechaLabor
                {
                    Id = 4,
                    CodigoLabor = "COS-2026-043",
                    LoteNombre = "Los Naranjales - Cuadro A",
                    TipoCultivo = "Limón",
                    HectareasLabor = 95,
                    FechaPlanificada = DateTime.Now.AddDays(-4),
                    FechaInicio = DateTime.Now.AddDays(-4),
                    ToneladasEstimadas = 2280,
                    ToneladasRecolectadas = 860,
                    MaquinariaAsignada = "Cuadrilla Manual 45 cosecheros + Tractores con bins",
                    CuadrillaResponsable = "Cuadrilla Citrícola Tafí",
                    Estado = "En Curso",
                    PorcentajeAvance = 38,
                    SiloDestinoSugerido = "Planta de Empaque y Frío Tafí Viejo"
                }
            };
        }

        private static List<TransporteViaje> CargarViajesIniciales()
        {
            return new List<TransporteViaje>
            {
                new TransporteViaje
                {
                    Id = 1,
                    CartaPorteNumero = "CTG-9048-2849102",
                    PatenteCamion = "AF-302-XY",
                    PatenteAcoplado = "AD-991-ZZ",
                    ChoferNombre = "Marcos Argañaraz",
                    ChoferDni = "28.491.022",
                    EmpresaTransporte = "Expreso Norte Logística SRL",
                    OrigenCampo = "Lote El Manantial (Leales, Tucumán)",
                    DestinoAcopio = "Planta de Acopio SiTech - Banda Río Salí",
                    GranoTransportado = "Soja",
                    PesoNetoTn = 32.5,
                    FechaSalida = DateTime.Now.AddHours(-3),
                    FechaLlegadaEstimada = DateTime.Now.AddMinutes(45),
                    Estado = "En Tránsito",
                    ProgresoRuta = 75,
                    Observaciones = "Carta de porte validada AFIP/SENASA. Humedad precarga: 13.0%."
                },
                new TransporteViaje
                {
                    Id = 2,
                    CartaPorteNumero = "CTG-9048-2849103",
                    PatenteCamion = "AE-812-MN",
                    PatenteAcoplado = "AC-112-OP",
                    ChoferNombre = "Cristian Herrera",
                    ChoferDni = "31.042.887",
                    EmpresaTransporte = "Transporte Cargas del NOA",
                    OrigenCampo = "Lote Cañon de Anta (Joaquín V. González, Salta)",
                    DestinoAcopio = "Terminal Portuaria 6 - Puerto General San Martín (Rosario)",
                    GranoTransportado = "Soja",
                    PesoNetoTn = 31.8,
                    FechaSalida = DateTime.Now.AddHours(-10),
                    FechaLlegadaEstimada = DateTime.Now.AddHours(6),
                    Estado = "En Tránsito",
                    ProgresoRuta = 60,
                    Observaciones = "Flete de larga distancia por Ruta Nacional 34. Sin novedades mecánicas."
                },
                new TransporteViaje
                {
                    Id = 3,
                    CartaPorteNumero = "CTG-9048-2849104",
                    PatenteCamion = "AG-115-KK",
                    PatenteAcoplado = "AA-765-LL",
                    ChoferNombre = "Ramón Díaz",
                    ChoferDni = "24.912.774",
                    EmpresaTransporte = "Flota Propia SiTech / Don Juan",
                    OrigenCampo = "Lote El Manantial Norte",
                    DestinoAcopio = "Planta de Acopio SiTech - Silo #1",
                    GranoTransportado = "Soja",
                    PesoNetoTn = 33.1,
                    FechaSalida = DateTime.Now.AddMinutes(-30),
                    FechaLlegadaEstimada = DateTime.Now.AddHours(2),
                    Estado = "En Balanza Origen",
                    ProgresoRuta = 15,
                    Observaciones = "Registrando pesaje bruto en báscula electrónica de campo."
                },
                new TransporteViaje
                {
                    Id = 4,
                    CartaPorteNumero = "CTG-9048-2849101",
                    PatenteCamion = "AC-432-PP",
                    PatenteAcoplado = "AD-554-QR",
                    ChoferNombre = "Esteban Morales",
                    ChoferDni = "29.883.190",
                    EmpresaTransporte = "TransGranos SRL",
                    OrigenCampo = "Lote El Manantial Norte",
                    DestinoAcopio = "Planta de Acopio SiTech - Silo #1",
                    GranoTransportado = "Soja",
                    PesoNetoTn = 30.9,
                    FechaSalida = DateTime.Now.AddHours(-6),
                    FechaLlegadaEstimada = DateTime.Now.AddHours(-4),
                    Estado = "Descargado",
                    ProgresoRuta = 100,
                    Observaciones = "Descarga en fosa 2 completada con éxito. Calidad certificada Grado 1."
                }
            };
        }

        private static List<AcopioSilo> CargarSilosIniciales()
        {
            return new List<AcopioSilo>
            {
                new AcopioSilo
                {
                    Id = 1,
                    Identificador = "Silo Chapa #01",
                    PlantaAcopio = "Planta Este - Banda del Río Salí",
                    TipoEstructura = "Silo Vertical Chapa Cónica",
                    GranoAlmacenado = "Soja",
                    CapacidadMaximaTn = 5000,
                    StockActualTn = 4120,
                    HumedadGranoPorcentaje = 12.8,
                    TemperaturaGranoC = 21.0,
                    EstadoAireacion = "Normal",
                    UltimaInspeccion = DateTime.Now.AddDays(-1),
                    CalidadGrano = "Grado 1 (Exportación)"
                },
                new AcopioSilo
                {
                    Id = 2,
                    Identificador = "Silo Chapa #02",
                    PlantaAcopio = "Planta Este - Banda del Río Salí",
                    TipoEstructura = "Silo Vertical Chapa Plana",
                    GranoAlmacenado = "Maíz",
                    CapacidadMaximaTn = 5000,
                    StockActualTn = 2750,
                    HumedadGranoPorcentaje = 13.9,
                    TemperaturaGranoC = 22.8,
                    EstadoAireacion = "Aireación Activa",
                    UltimaInspeccion = DateTime.Now.AddDays(-2),
                    CalidadGrano = "Grado 2"
                },
                new AcopioSilo
                {
                    Id = 3,
                    Identificador = "Silo Chapa #03",
                    PlantaAcopio = "Planta Este - Banda del Río Salí",
                    TipoEstructura = "Silo Vertical Chapa Cónica",
                    GranoAlmacenado = "Trigo Pan",
                    CapacidadMaximaTn = 3500,
                    StockActualTn = 1150,
                    HumedadGranoPorcentaje = 12.1,
                    TemperaturaGranoC = 19.5,
                    EstadoAireacion = "Normal",
                    UltimaInspeccion = DateTime.Now.AddDays(-4),
                    CalidadGrano = "Grado 1"
                },
                new AcopioSilo
                {
                    Id = 4,
                    Identificador = "Silo-Bolsa Lote Anta A-1",
                    PlantaAcopio = "Centro de Acopio de Campaña Anta",
                    TipoEstructura = "Silo Bolsa Silomax 9 pies",
                    GranoAlmacenado = "Soja",
                    CapacidadMaximaTn = 250,
                    StockActualTn = 250,
                    HumedadGranoPorcentaje = 13.0,
                    TemperaturaGranoC = 20.2,
                    EstadoAireacion = "Normal",
                    UltimaInspeccion = DateTime.Now.AddDays(-1),
                    CalidadGrano = "Grado 1"
                },
                new AcopioSilo
                {
                    Id = 5,
                    Identificador = "Silo-Bolsa Lote Anta A-2",
                    PlantaAcopio = "Centro de Acopio de Campaña Anta",
                    TipoEstructura = "Silo Bolsa Silomax 9 pies",
                    GranoAlmacenado = "Soja",
                    CapacidadMaximaTn = 250,
                    StockActualTn = 190,
                    HumedadGranoPorcentaje = 13.2,
                    TemperaturaGranoC = 20.8,
                    EstadoAireacion = "Normal",
                    UltimaInspeccion = DateTime.Now.AddDays(-1),
                    CalidadGrano = "Grado 1"
                }
            };
        }

        private static List<EstacionClimatica> CargarEstacionesIniciales()
        {
            return new List<EstacionClimatica>
            {
                new EstacionClimatica
                {
                    Id = 1,
                    NombreEstacion = "Estación Tafí Viejo (Citrícola)",
                    Provincia = "Tucumán",
                    TemperaturaC = 23.8,
                    SensacionTermicaC = 24.5,
                    HumedadRelativaPorcentaje = 66,
                    PuntoRocioC = 17.1,
                    VientoVelocidadKmH = 12,
                    RafagasKmH = 18,
                    VientoDireccion = "SE",
                    PrecipitacionUltimas24hMm = 4.2,
                    LluviaAcumuladaMesMm = 72.4,
                    EvapotranspiracionEtoMm = 3.8,
                    PresionHpa = 1013,
                    EstadoCielo = "Parcialmente Nublado",
                    IndiceUV = "Moderado (5)",
                    RadiacionSolarWm2 = 680,
                    DeltaTC = 4.2,
                    CondicionPulverizacion = "Ventana Óptima",
                    DiagnosticoVentana = "Condiciones muy favorables para pulverización. Delta T en rango seguro (2-8°C), deriva baja.",
                    PronosticoExtendido = new List<PronosticoDia>
                    {
                        new PronosticoDia { DiaSemana = "Hoy", FechaCorta = "18 Sep", TempMinC = 16, TempMaxC = 24, Condicion = "Parcial Nublado", Icono = "⛅", ProbabilidadLluviaPorcentaje = 15, VientoMaxKmH = 15, EstadoVentana = "Óptima" },
                        new PronosticoDia { DiaSemana = "Viernes", FechaCorta = "19 Sep", TempMinC = 17, TempMaxC = 26, Condicion = "Soleado", Icono = "☀️", ProbabilidadLluviaPorcentaje = 5, VientoMaxKmH = 14, EstadoVentana = "Óptima" },
                        new PronosticoDia { DiaSemana = "Sábado", FechaCorta = "20 Sep", TempMinC = 19, TempMaxC = 28, Condicion = "Cálido", Icono = "🌤️", ProbabilidadLluviaPorcentaje = 10, VientoMaxKmH = 18, EstadoVentana = "Precaución" },
                        new PronosticoDia { DiaSemana = "Domingo", FechaCorta = "21 Sep", TempMinC = 18, TempMaxC = 25, Condicion = "Tormentas Aisladas", Icono = "⛈️", ProbabilidadLluviaPorcentaje = 65, VientoMaxKmH = 26, EstadoVentana = "No Recomendada" },
                        new PronosticoDia { DiaSemana = "Lunes", FechaCorta = "22 Sep", TempMinC = 15, TempMaxC = 22, Condicion = "Lloviznas Leves", Icono = "🌦️", ProbabilidadLluviaPorcentaje = 40, VientoMaxKmH = 16, EstadoVentana = "No Recomendada" }
                    }
                },
                new EstacionClimatica
                {
                    Id = 2,
                    NombreEstacion = "Estación Leales (Llanura Granaria)",
                    Provincia = "Tucumán",
                    TemperaturaC = 26.5,
                    SensacionTermicaC = 27.2,
                    HumedadRelativaPorcentaje = 58,
                    PuntoRocioC = 17.5,
                    VientoVelocidadKmH = 15,
                    RafagasKmH = 22,
                    VientoDireccion = "E",
                    PrecipitacionUltimas24hMm = 0.0,
                    LluviaAcumuladaMesMm = 54.0,
                    EvapotranspiracionEtoMm = 4.6,
                    PresionHpa = 1012,
                    EstadoCielo = "Soleado / Despejado",
                    IndiceUV = "Alto (7)",
                    RadiacionSolarWm2 = 790,
                    DeltaTC = 5.6,
                    CondicionPulverizacion = "Ventana Óptima",
                    DiagnosticoVentana = "Apta para tratamientos fitosanitarios hasta las 11:30 hs y posterior a las 16:30 hs.",
                    PronosticoExtendido = new List<PronosticoDia>
                    {
                        new PronosticoDia { DiaSemana = "Hoy", FechaCorta = "18 Sep", TempMinC = 15, TempMaxC = 27, Condicion = "Despejado", Icono = "☀️", ProbabilidadLluviaPorcentaje = 0, VientoMaxKmH = 16, EstadoVentana = "Óptima" },
                        new PronosticoDia { DiaSemana = "Viernes", FechaCorta = "19 Sep", TempMinC = 16, TempMaxC = 29, Condicion = "Soleado", Icono = "☀️", ProbabilidadLluviaPorcentaje = 5, VientoMaxKmH = 15, EstadoVentana = "Óptima" },
                        new PronosticoDia { DiaSemana = "Sábado", FechaCorta = "20 Sep", TempMinC = 18, TempMaxC = 31, Condicion = "Viento Norte", Icono = "🌤️", ProbabilidadLluviaPorcentaje = 10, VientoMaxKmH = 22, EstadoVentana = "Precaución" },
                        new PronosticoDia { DiaSemana = "Domingo", FechaCorta = "21 Sep", TempMinC = 19, TempMaxC = 26, Condicion = "Chubascos", Icono = "🌧️", ProbabilidadLluviaPorcentaje = 70, VientoMaxKmH = 28, EstadoVentana = "No Recomendada" },
                        new PronosticoDia { DiaSemana = "Lunes", FechaCorta = "22 Sep", TempMinC = 14, TempMaxC = 23, Condicion = "Fresco / Nublado", Icono = "⛅", ProbabilidadLluviaPorcentaje = 20, VientoMaxKmH = 14, EstadoVentana = "Óptima" }
                    }
                },
                new EstacionClimatica
                {
                    Id = 3,
                    NombreEstacion = "Estación J.V. González (Anta)",
                    Provincia = "Salta",
                    TemperaturaC = 29.2,
                    SensacionTermicaC = 29.8,
                    HumedadRelativaPorcentaje = 41,
                    PuntoRocioC = 14.8,
                    VientoVelocidadKmH = 22,
                    RafagasKmH = 34,
                    VientoDireccion = "N",
                    PrecipitacionUltimas24hMm = 0.0,
                    LluviaAcumuladaMesMm = 38.0,
                    EvapotranspiracionEtoMm = 5.8,
                    PresionHpa = 1010,
                    EstadoCielo = "Despejado con Viento",
                    IndiceUV = "Muy Alto (9)",
                    RadiacionSolarWm2 = 880,
                    DeltaTC = 8.6,
                    CondicionPulverizacion = "Precaución / Límite",
                    DiagnosticoVentana = "Delta T elevado (>8°C) y viento > 20 km/h. Riesgo de evaporación rápida de gotas y deriva moderada.",
                    PronosticoExtendido = new List<PronosticoDia>
                    {
                        new PronosticoDia { DiaSemana = "Hoy", FechaCorta = "18 Sep", TempMinC = 18, TempMaxC = 30, Condicion = "Caluroso / Ventoso", Icono = "🌤️", ProbabilidadLluviaPorcentaje = 0, VientoMaxKmH = 24, EstadoVentana = "Precaución" },
                        new PronosticoDia { DiaSemana = "Viernes", FechaCorta = "19 Sep", TempMinC = 19, TempMaxC = 33, Condicion = "Viento Zonda", Icono = "☀️", ProbabilidadLluviaPorcentaje = 0, VientoMaxKmH = 32, EstadoVentana = "No Recomendada" },
                        new PronosticoDia { DiaSemana = "Sábado", FechaCorta = "20 Sep", TempMinC = 20, TempMaxC = 34, Condicion = "Muy Caluroso", Icono = "🔥", ProbabilidadLluviaPorcentaje = 5, VientoMaxKmH = 25, EstadoVentana = "Precaución" },
                        new PronosticoDia { DiaSemana = "Domingo", FechaCorta = "21 Sep", TempMinC = 17, TempMaxC = 24, Condicion = "Frente Frío / Tormenta", Icono = "⛈️", ProbabilidadLluviaPorcentaje = 80, VientoMaxKmH = 38, EstadoVentana = "No Recomendada" },
                        new PronosticoDia { DiaSemana = "Lunes", FechaCorta = "22 Sep", TempMinC = 13, TempMaxC = 22, Condicion = "Despejado", Icono = "☀️", ProbabilidadLluviaPorcentaje = 10, VientoMaxKmH = 12, EstadoVentana = "Óptima" }
                    }
                },
                new EstacionClimatica
                {
                    Id = 4,
                    NombreEstacion = "Estación Rosario de la Frontera",
                    Provincia = "Salta",
                    TemperaturaC = 24.6,
                    SensacionTermicaC = 25.1,
                    HumedadRelativaPorcentaje = 52,
                    PuntoRocioC = 14.2,
                    VientoVelocidadKmH = 10,
                    RafagasKmH = 16,
                    VientoDireccion = "NE",
                    PrecipitacionUltimas24hMm = 1.0,
                    LluviaAcumuladaMesMm = 46.5,
                    EvapotranspiracionEtoMm = 4.0,
                    PresionHpa = 1014,
                    EstadoCielo = "Despejado",
                    IndiceUV = "Moderado (6)",
                    RadiacionSolarWm2 = 710,
                    DeltaTC = 5.1,
                    CondicionPulverizacion = "Ventana Óptima",
                    DiagnosticoVentana = "Condiciones atmosféricas sumamente estables. Pulverizaciones y cosecha en ventanas normales.",
                    PronosticoExtendido = new List<PronosticoDia>
                    {
                        new PronosticoDia { DiaSemana = "Hoy", FechaCorta = "18 Sep", TempMinC = 14, TempMaxC = 25, Condicion = "Despejado", Icono = "☀️", ProbabilidadLluviaPorcentaje = 0, VientoMaxKmH = 12, EstadoVentana = "Óptima" },
                        new PronosticoDia { DiaSemana = "Viernes", FechaCorta = "19 Sep", TempMinC = 15, TempMaxC = 27, Condicion = "Soleado", Icono = "☀️", ProbabilidadLluviaPorcentaje = 0, VientoMaxKmH = 14, EstadoVentana = "Óptima" },
                        new PronosticoDia { DiaSemana = "Sábado", FechaCorta = "20 Sep", TempMinC = 17, TempMaxC = 30, Condicion = "Nubosidad Variable", Icono = "⛅", ProbabilidadLluviaPorcentaje = 15, VientoMaxKmH = 16, EstadoVentana = "Óptima" },
                        new PronosticoDia { DiaSemana = "Domingo", FechaCorta = "21 Sep", TempMinC = 16, TempMaxC = 23, Condicion = "Lluvias Aisladas", Icono = "🌧️", ProbabilidadLluviaPorcentaje = 60, VientoMaxKmH = 22, EstadoVentana = "No Recomendada" },
                        new PronosticoDia { DiaSemana = "Lunes", FechaCorta = "22 Sep", TempMinC = 12, TempMaxC = 21, Condicion = "Fresco / Soleado", Icono = "☀️", ProbabilidadLluviaPorcentaje = 5, VientoMaxKmH = 10, EstadoVentana = "Óptima" }
                    }
                }
            };
        }

        private static List<AlertaMeteorologica> CargarAlertasIniciales()
        {
            return new List<AlertaMeteorologica>
            {
                new AlertaMeteorologica
                {
                    Id = 1,
                    Titulo = "Alerta Meteorológica: Tormentas Severas y Probabilidad de Granizo",
                    ModuloOrigen = "Clima",
                    TipoAlerta = "Tormenta Severa",
                    NivelSeveridad = "Crítica",
                    ZonaAfectada = "Este de Tucumán (Cruz Alta, Leales, Burruyacú)",
                    Descripcion = "Frente convectivo severo con ráfagas superiores a 60 km/h y precipitaciones estimadas en 40-60 mm en las próximas 4 horas.",
                    RecomendacionOperativa = "Cubrir inmediatamente tolvas y camiones con lona impermeable; suspender labores de trilla abierta.",
                    FechaEmision = DateTime.Now.AddMinutes(-35),
                    Activa = true
                },
                new AlertaMeteorologica
                {
                    Id = 2,
                    Titulo = "Alerta de Acopio: Incremento Térmico en Silo Chapa #02",
                    ModuloOrigen = "Acopio",
                    TipoAlerta = "Temperatura Grano",
                    NivelSeveridad = "Crítica",
                    ZonaAfectada = "Planta Este - Banda del Río Salí (Silo #02)",
                    Descripcion = "Termometría registra 28.5 °C en estrato medio con humedad de 13.9% en masa de grano de maíz.",
                    RecomendacionOperativa = "Activar inmediatamente turbinas de aireación forzada y ciclo de enfriamiento nocturno.",
                    FechaEmision = DateTime.Now.AddHours(-1),
                    Activa = true
                },
                new AlertaMeteorologica
                {
                    Id = 3,
                    Titulo = "Alerta Fitosanitaria: Incidencia de Oruga Cogollera (Spodoptera)",
                    ModuloOrigen = "Cultivos",
                    TipoAlerta = "Plaga Fitosanitaria",
                    NivelSeveridad = "Alta",
                    ZonaAfectada = "Lote LT-NOA-003 (Finca El Rodeo - Anta, Salta)",
                    Descripcion = "Captura de trampas de luz y monitoreo en campo arrojan 22% de plantas con daño fresco en cogollo de maíz tardío.",
                    RecomendacionOperativa = "Programar pulverizadora autopropulsada con insecticida selectivo de bajo impacto ambiental dentro de las 48 hs.",
                    FechaEmision = DateTime.Now.AddHours(-2),
                    Activa = true
                },
                new AlertaMeteorologica
                {
                    Id = 4,
                    Titulo = "Alerta Logística: Demora y Detención en Ruta Nacional 34",
                    ModuloOrigen = "Transporte",
                    TipoAlerta = "Tránsito y Flota",
                    NivelSeveridad = "Media",
                    ZonaAfectada = "Ruta Nac. 34 Km 782 (Rosario de la Frontera)",
                    Descripcion = "Unidad Scania R450 (Chofer R. Gómez - CP-2026-08812) detenida por más de 50 minutos con 32 Tn de Soja hacia acopio.",
                    RecomendacionOperativa = "Contactar al chofer mediante telemetría satelital y constatar estado de la carga y hoja de ruta.",
                    FechaEmision = DateTime.Now.AddHours(-3),
                    Activa = true
                },
                new AlertaMeteorologica
                {
                    Id = 5,
                    Titulo = "Alerta de Cosecha: Saturación de Capacidad en Tolvas Autodescargables",
                    ModuloOrigen = "Cosecha",
                    TipoAlerta = "Capacidad Operativa",
                    NivelSeveridad = "Alta",
                    ZonaAfectada = "Finca Santa Rosa - Lote El Manantial (Leales, Tucumán)",
                    Descripcion = "Ritmo de recolección de cosechadoras John Deere supera capacidad de descarga. Tolvas al 95% de capacidad.",
                    RecomendacionOperativa = "Asignar 2 camiones de auxilio logístico con prioridad de acceso a cabecera de lote.",
                    FechaEmision = DateTime.Now.AddHours(-4),
                    Activa = true
                },
                new AlertaMeteorologica
                {
                    Id = 6,
                    Titulo = "Validación Pendiente de Certificado SENASA / CTG para Despacho",
                    ModuloOrigen = "Transporte",
                    TipoAlerta = "Conformidad y Calidad",
                    NivelSeveridad = "Informativa",
                    ZonaAfectada = "Despacho Finca Bella Vista -> Puerto Rosario",
                    Descripcion = "Carga de granos lista para despacho requiere confirmación de análisis fitosanitario e integración fiscal CTG.",
                    RecomendacionOperativa = "Validar el CTG en AFIP antes de liberar la salida del transporte de carga pesada.",
                    FechaEmision = DateTime.Now.AddHours(-5),
                    Activa = true
                },
                new AlertaMeteorologica
                {
                    Id = 7,
                    Titulo = "Aviso Preventivo: Viento Zonda en Zonas de Precordillera",
                    ModuloOrigen = "Clima",
                    TipoAlerta = "Viento Fuerte",
                    NivelSeveridad = "Media",
                    ZonaAfectada = "Valles Calchaquíes y Piedemonte",
                    Descripcion = "Ráfagas secas superiores a 55 km/h con marcado descenso de humedad relativa (<20%).",
                    RecomendacionOperativa = "Extremar prevención contra incendios rurales. Prohibir quemas de rastrojo.",
                    FechaEmision = DateTime.Now.AddDays(-1),
                    Activa = false
                }
            };
        }

        private static List<TrazabilidadItem> CargarTrazabilidadesIniciales()
        {
            return new List<TrazabilidadItem>
            {
                new TrazabilidadItem
                {
                    Id = 1,
                    CodigoTrazabilidad = "TRZ-2026-SOJ-0914",
                    NombrePartida = "Partida Soja Primera - Campaña 2026",
                    Cultivo = "Soja",
                    LoteOrigen = "LT-LEAL-01 (Lote El Manantial Norte)",
                    Establecimiento = "Finca Santa Rosa - Leales, Tucumán",
                    ProductorCliente = "AgroNorte S.A.",
                    ToneladasTotales = 540,
                    CalidadCertificada = "Grado 1 - Libre OGM - 12.8% Humedad",
                    SiloUbicacionActual = "Silo Chapa #01 (Banda Río Salí)",
                    DestinoFinalPrevisto = "Terminal Puerto Rosario / Molienda Exportación",
                    EstadoCadena = "En Acopio",
                    FechaCreacion = DateTime.Now.AddDays(-2),
                    HistorialHitos = new List<HitoTrazabilidad>
                    {
                        new HitoTrazabilidad
                        {
                            Paso = 1,
                            Titulo = "Siembra Georreferenciada",
                            Descripcion = "Siembra con semilla certificada DM 46i20 con densidad de 320.000 pl/ha.",
                            FechaHora = DateTime.Now.AddMonths(-4),
                            ResponsableOEntidad = "Ing. Germán Paz (SiTech Campo)",
                            Ubicacion = "Finca Santa Rosa, Leales",
                            DocumentoAsociado = "CERT-SIEMBRA-2025-1102"
                        },
                        new HitoTrazabilidad
                        {
                            Paso = 2,
                            Titulo = "Monitoreo Satelital y Fitosanitario",
                            Descripcion = "Monitoreo con índice NDVI 0.81 y aplicación de fungicida preventivo.",
                            FechaHora = DateTime.Now.AddMonths(-1),
                            ResponsableOEntidad = "Sistema SiTech AgroSat",
                            Ubicacion = "Coordenadas -27.128, -65.192",
                            DocumentoAsociado = "RPT-NDVI-0419"
                        },
                        new HitoTrazabilidad
                        {
                            Paso = 3,
                            Titulo = "Labor de Cosecha Mecanizada",
                            Descripcion = "Cosecha con John Deere S780 equipada con monitor de rinde digital.",
                            FechaHora = DateTime.Now.AddDays(-2),
                            ResponsableOEntidad = "Equipo Cosecha Alfa (Marcelo Gómez)",
                            Ubicacion = "Lote El Manantial Norte",
                            DocumentoAsociado = "COS-2026-041"
                        },
                        new HitoTrazabilidad
                        {
                            Paso = 4,
                            Titulo = "Pesaje de Balanza y Carta de Porte Electrónica",
                            Descripcion = "Pesaje en báscula calibrada y emisión de CTG Electrónica autorizada por ARCA/AFIP.",
                            FechaHora = DateTime.Now.AddDays(-1),
                            ResponsableOEntidad = "Operador Balanza SiTech",
                            Ubicacion = "Balanza Campo Santa Rosa",
                            DocumentoAsociado = "CTG-9048-2849102"
                        },
                        new HitoTrazabilidad
                        {
                            Paso = 5,
                            Titulo = "Ingreso a Silo de Acopio y Calidad",
                            Descripcion = "Calado neumático, análisis de calidad (Grado 1, 12.8% humedad, 0.5% cuerpos extraños) y vaciado en fosa 1.",
                            FechaHora = DateTime.Now.AddHours(-12),
                            ResponsableOEntidad = "Laboratorio Calidad Banda Río Salí",
                            Ubicacion = "Planta Este - Silo Chapa #01",
                            DocumentoAsociado = "TICKET-CALIDAD-8821"
                        }
                    }
                },
                new TrazabilidadItem
                {
                    Id = 2,
                    CodigoTrazabilidad = "TRZ-2026-SOJ-0902",
                    NombrePartida = "Partida Soja Salteña Exportación",
                    Cultivo = "Soja",
                    LoteOrigen = "LT-ANTA-04 (Cañon de Anta)",
                    Establecimiento = "Establecimiento Don Juan - Salta",
                    ProductorCliente = "Agrícola El Trébol S.R.L.",
                    ToneladasTotales = 1680,
                    CalidadCertificada = "Grado 1 Standard Exportación",
                    SiloUbicacionActual = "En Tránsito Flete Ferroviario / Camiones",
                    DestinoFinalPrevisto = "Puerto General San Martín (Rosario)",
                    EstadoCadena = "En Tránsito",
                    FechaCreacion = DateTime.Now.AddDays(-5),
                    HistorialHitos = new List<HitoTrazabilidad>
                    {
                        new HitoTrazabilidad
                        {
                            Paso = 1,
                            Titulo = "Siembra y Fertilización",
                            Descripcion = "Siembra directa con fertilización fosforada.",
                            FechaHora = DateTime.Now.AddMonths(-5),
                            ResponsableOEntidad = "Agrícola El Trébol",
                            Ubicacion = "Joaquín V. González, Salta",
                            DocumentoAsociado = "ORD-AGRO-772"
                        },
                        new HitoTrazabilidad
                        {
                            Paso = 2,
                            Titulo = "Cosecha Completa de 520 Ha",
                            Descripcion = "Rinde promedio obtenido 3.23 Tn/Ha con humedad de 13.0%.",
                            FechaHora = DateTime.Now.AddDays(-3),
                            ResponsableOEntidad = "Contratista Salta Cosechas",
                            Ubicacion = "Lote Cañon de Anta",
                            DocumentoAsociado = "COS-2026-039"
                        },
                        new HitoTrazabilidad
                        {
                            Paso = 3,
                            Titulo = "Despacho Flete a Puerto Rosario",
                            Descripcion = "Despacho de convoy de 15 camiones con Cartas de Porte Electrónicas individuales.",
                            FechaHora = DateTime.Now.AddHours(-10),
                            ResponsableOEntidad = "Logística SiTech NOA",
                            Ubicacion = "Ruta Nac. 34 rumbo a Santa Fe",
                            DocumentoAsociado = "CTG-9048-2849103"
                        }
                    }
                }
            };
        }

        private static List<ClienteProductor> CargarClientesIniciales()
        {
            return new List<ClienteProductor>
            {
                new ClienteProductor
                {
                    Id = 1,
                    RazonSocial = "AgroNorte S.A.",
                    Cuit = "30-71289140-9",
                    TipoCliente = "Productor Agropecuario",
                    Localidad = "San Miguel de Tucumán",
                    Provincia = "Tucumán",
                    HectareasOperadas = 3400,
                    ContactoNombre = "Ing. Federico Colombres",
                    Telefono = "+54 381 422-9011",
                    Email = "fcolombres@agronorte.com.ar",
                    Estado = "Activo",
                    ContratosVigentes = 3
                },
                new ClienteProductor
                {
                    Id = 2,
                    RazonSocial = "Agrícola El Trébol S.R.L.",
                    Cuit = "33-68420199-9",
                    TipoCliente = "Productor Agropecuario",
                    Localidad = "Joaquín V. González",
                    Provincia = "Salta",
                    HectareasOperadas = 5200,
                    ContactoNombre = "Mariano Cornejo",
                    Telefono = "+54 387 491-0023",
                    Email = "mcornejo@eltrebolagro.com.ar",
                    Estado = "Activo",
                    ContratosVigentes = 2
                },
                new ClienteProductor
                {
                    Id = 3,
                    RazonSocial = "Cooperativa Agropecuaria Unión de Leales",
                    Cuit = "30-55410982-4",
                    TipoCliente = "Cooperativa Agrícola",
                    Localidad = "Leales",
                    Provincia = "Tucumán",
                    HectareasOperadas = 1800,
                    ContactoNombre = "Gustavo Terán",
                    Telefono = "+54 381 487-1155",
                    Email = "administracion@coopleales.com.ar",
                    Estado = "Activo",
                    ContratosVigentes = 4
                },
                new ClienteProductor
                {
                    Id = 4,
                    RazonSocial = "Citromax Productores Asociados",
                    Cuit = "30-61099234-1",
                    TipoCliente = "Industria & Productor",
                    Localidad = "Tafí Viejo",
                    Provincia = "Tucumán",
                    HectareasOperadas = 920,
                    ContactoNombre = "Lic. Andrea Padilla",
                    Telefono = "+54 381 498-3320",
                    Email = "apadilla@citromaxpro.com.ar",
                    Estado = "Activo",
                    ContratosVigentes = 2
                },
                new ClienteProductor
                {
                    Id = 5,
                    RazonSocial = "Molinos & Granos del NOA S.A.",
                    Cuit = "30-70984122-3",
                    TipoCliente = "Industria Molienda / Acopiador",
                    Localidad = "Banda del Río Salí",
                    Provincia = "Tucumán",
                    HectareasOperadas = 0,
                    ContactoNombre = "Horacio Alperovich",
                    Telefono = "+54 381 430-8800",
                    Email = "operaciones@molinosnoa.com.ar",
                    Estado = "Activo",
                    ContratosVigentes = 5
                }
            };
        }

        private static List<CampanaProyecto> CargarCampanasIniciales()
        {
            return new List<CampanaProyecto>
            {
                new CampanaProyecto
                {
                    Id = 1,
                    Codigo = "CAMP-2025/26-GRUESA",
                    Nombre = "Campaña Gruesa 2025/2026 - Soja & Maíz NOA",
                    TipoCiclo = "Gruesa",
                    FechaInicio = new DateTime(2025, 10, 15),
                    FechaEstimadaCierre = new DateTime(2026, 5, 30),
                    ResponsableGeneral = "Ing. Germán Paz",
                    HectareasPlanificadas = 11320,
                    ToneladasObjetivo = 38500,
                    PresupuestoEstimadoUsd = 1250000,
                    AvancePorcentaje = 62,
                    Estado = "En Curso"
                },
                new CampanaProyecto
                {
                    Id = 2,
                    Codigo = "CAMP-2026-ZAFRA",
                    Nombre = "Campaña Azucarera - Zafra 2026",
                    TipoCiclo = "Zafra Azucarera",
                    FechaInicio = new DateTime(2026, 5, 1),
                    FechaEstimadaCierre = new DateTime(2026, 11, 15),
                    ResponsableGeneral = "Ing. Carlos Frías",
                    HectareasPlanificadas = 4500,
                    ToneladasObjetivo = 337500,
                    PresupuestoEstimadoUsd = 2100000,
                    AvancePorcentaje = 25,
                    Estado = "En Curso"
                },
                new CampanaProyecto
                {
                    Id = 3,
                    Codigo = "CAMP-2026-CITRICOS",
                    Nombre = "Temporada Cítricos 2026 - Limón NOA Export",
                    TipoCiclo = "Temporada Cítrica",
                    FechaInicio = new DateTime(2026, 2, 1),
                    FechaEstimadaCierre = new DateTime(2026, 8, 30),
                    ResponsableGeneral = "Lic. Sofía Albarracín",
                    HectareasPlanificadas = 1200,
                    ToneladasObjetivo = 28000,
                    PresupuestoEstimadoUsd = 890000,
                    AvancePorcentaje = 40,
                    Estado = "En Curso"
                },
                new CampanaProyecto
                {
                    Id = 4,
                    Codigo = "CAMP-2026-FINA",
                    Nombre = "Campaña Fina 2026 - Trigo & Legumbres",
                    TipoCiclo = "Fina",
                    FechaInicio = new DateTime(2026, 6, 1),
                    FechaEstimadaCierre = new DateTime(2026, 12, 20),
                    ResponsableGeneral = "Ing. Germán Paz",
                    HectareasPlanificadas = 3100,
                    ToneladasObjetivo = 7500,
                    PresupuestoEstimadoUsd = 480000,
                    AvancePorcentaje = 0,
                    Estado = "Planificada"
                }
            };
        }

        private static List<Empleado> CargarEmpleadosIniciales()
        {
            return new List<Empleado>
            {
                new Empleado
                {
                    Id = 1,
                    Legajo = "LEG-0104",
                    NombreCompleto = "Ing. Germán Paz",
                    Dni = "25.198.402",
                    PuestoRol = "Ingeniero Agrónomo Supervisor",
                    Departamento = "Operaciones Agronómicas",
                    Telefono = "+54 381 512-3901",
                    Email = "gpaz@sitech.com.ar",
                    EstadoDisponibilidad = "Disponible",
                    UbicacionActual = "Base Central Leales"
                },
                new Empleado
                {
                    Id = 2,
                    Legajo = "LEG-0209",
                    NombreCompleto = "Marcos Argañaraz",
                    Dni = "28.491.022",
                    PuestoRol = "Chofer Logístico de Carga Pesada",
                    Departamento = "Logística y Flota",
                    Telefono = "+54 381 477-8812",
                    Email = "margañaraz@expresonorte.com.ar",
                    EstadoDisponibilidad = "En Tránsito",
                    UbicacionActual = "Ruta Prov. 306 km 18"
                },
                new Empleado
                {
                    Id = 3,
                    Legajo = "LEG-0211",
                    NombreCompleto = "Cristian Herrera",
                    Dni = "31.042.887",
                    PuestoRol = "Chofer Logístico de Carga Pesada",
                    Departamento = "Logística y Flota",
                    Telefono = "+54 387 501-4490",
                    Email = "cherrera@transnoa.com.ar",
                    EstadoDisponibilidad = "En Tránsito",
                    UbicacionActual = "Ruta Nac. 34 rumbo a Rosario"
                },
                new Empleado
                {
                    Id = 4,
                    Legajo = "LEG-0305",
                    NombreCompleto = "Marcelo Gómez",
                    Dni = "33.784.112",
                    PuestoRol = "Operador de Cosechadora y Maquinaria",
                    Departamento = "Operaciones Agronómicas",
                    Telefono = "+54 381 620-1922",
                    Email = "mgomez@sitech.com.ar",
                    EstadoDisponibilidad = "Asignado a Cosecha",
                    UbicacionActual = "Lote El Manantial Norte"
                },
                new Empleado
                {
                    Id = 5,
                    Legajo = "LEG-0402",
                    NombreCompleto = "Rodrigo Juárez",
                    Dni = "35.109.843",
                    PuestoRol = "Operador Balanza & Analista de Granos",
                    Departamento = "Calidad y Acopio",
                    Telefono = "+54 381 409-2211",
                    Email = "rjuarez@sitech.com.ar",
                    EstadoDisponibilidad = "Disponible",
                    UbicacionActual = "Planta Este - Banda Río Salí"
                },
                new Empleado
                {
                    Id = 6,
                    Legajo = "LEG-0408",
                    NombreCompleto = "Ing. Laura Beltrán",
                    Dni = "32.411.094",
                    PuestoRol = "Supervisora de Calidad e Inocuidad",
                    Departamento = "Calidad y Acopio",
                    Telefono = "+54 381 588-4100",
                    Email = "lbeltran@sitech.com.ar",
                    EstadoDisponibilidad = "Disponible",
                    UbicacionActual = "Laboratorio Central SiTech"
                }
            };
        }
        #endregion
    }
}
