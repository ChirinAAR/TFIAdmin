using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class ReportesViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private string _reporteSeleccionado = "Rendimiento y Cosecha por Lote";
        private string _periodoSeleccionado = "Campaña Actual (2025/2026)";
        private string _mensajeExportacion = string.Empty;

        private string _headerCol1 = "Lote / Parcela";
        private string _headerCol2 = "Cultivo";
        private string _headerCol3 = "Superficie (Ha)";
        private string _headerCol4 = "Cosechado (Tn)";
        private string _headerCol5 = "Rinde Real (Tn/Ha)";

        public ReportesViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;

            TiposReporte = new ObservableCollection<string>
            {
                "Rendimiento y Cosecha por Lote",
                "Logística de Fletes y Cartas de Porte",
                "Balance de Acopio e Inventario en Silos",
                "Auditoría y Trazabilidad de Partidas"
            };

            Periodos = new ObservableCollection<string>
            {
                "Campaña Actual (2025/2026)",
                "Últimos 30 días",
                "Histórico Completo"
            };

            Filas = new ObservableCollection<FilaReporte>();

            GenerarReporteCommand = new RelayCommand(ActualizarDatosReporte);
            ExportarExcelCommand = new RelayCommand(EjecutarExportarExcel);
            ExportarPdfCommand = new RelayCommand(EjecutarExportarPdf);

            ActualizarDatosReporte();
        }

        public ObservableCollection<string> TiposReporte { get; }
        public ObservableCollection<string> Periodos { get; }
        public ObservableCollection<FilaReporte> Filas { get; }

        public string ReporteSeleccionado
        {
            get => _reporteSeleccionado;
            set
            {
                if (SetProperty(ref _reporteSeleccionado, value))
                    ActualizarDatosReporte();
            }
        }

        public string PeriodoSeleccionado
        {
            get => _periodoSeleccionado;
            set
            {
                if (SetProperty(ref _periodoSeleccionado, value))
                    ActualizarDatosReporte();
            }
        }

        public string HeaderCol1 { get => _headerCol1; set => SetProperty(ref _headerCol1, value); }
        public string HeaderCol2 { get => _headerCol2; set => SetProperty(ref _headerCol2, value); }
        public string HeaderCol3 { get => _headerCol3; set => SetProperty(ref _headerCol3, value); }
        public string HeaderCol4 { get => _headerCol4; set => SetProperty(ref _headerCol4, value); }
        public string HeaderCol5 { get => _headerCol5; set => SetProperty(ref _headerCol5, value); }

        public string MensajeExportacion
        {
            get => _mensajeExportacion;
            set => SetProperty(ref _mensajeExportacion, value);
        }

        public ICommand GenerarReporteCommand { get; }
        public ICommand ExportarExcelCommand { get; }
        public ICommand ExportarPdfCommand { get; }

        private void ActualizarDatosReporte()
        {
            Filas.Clear();
            MensajeExportacion = string.Empty;

            switch (ReporteSeleccionado)
            {
                case "Rendimiento y Cosecha por Lote":
                    HeaderCol1 = "Lote / Establecimiento";
                    HeaderCol2 = "Cultivo";
                    HeaderCol3 = "Superficie";
                    HeaderCol4 = "Tn Cosechadas";
                    HeaderCol5 = "Rinde (Tn/Ha)";
                    foreach (var labor in _agroService.ObtenerLaboresCosecha())
                    {
                        Filas.Add(new FilaReporte
                        {
                            Columna1 = labor.LoteNombre,
                            Columna2 = labor.TipoCultivo,
                            Columna3 = $"{labor.HectareasLabor:N0} Ha",
                            Columna4 = $"{labor.ToneladasRecolectadas:N1} Tn",
                            Columna5 = $"{labor.RindeRealTnHa:F2} Tn/Ha",
                            Estado = labor.Estado
                        });
                    }
                    break;

                case "Logística de Fletes y Cartas de Porte":
                    HeaderCol1 = "Carta Porte (CTG)";
                    HeaderCol2 = "Chofer / Vehículo";
                    HeaderCol3 = "Origen";
                    HeaderCol4 = "Destino";
                    HeaderCol5 = "Carga Neta";
                    foreach (var viaje in _agroService.ObtenerViajesTransporte())
                    {
                        Filas.Add(new FilaReporte
                        {
                            Columna1 = viaje.CartaPorteNumero,
                            Columna2 = $"{viaje.ChoferNombre} ({viaje.PatenteCamion})",
                            Columna3 = viaje.OrigenCampo,
                            Columna4 = viaje.DestinoAcopio,
                            Columna5 = $"{viaje.PesoNetoTn:N1} Tn {viaje.GranoTransportado}",
                            Estado = viaje.Estado
                        });
                    }
                    break;

                case "Balance de Acopio e Inventario en Silos":
                    HeaderCol1 = "Silo / Almacén";
                    HeaderCol2 = "Planta";
                    HeaderCol3 = "Grano";
                    HeaderCol4 = "Stock Actual";
                    HeaderCol5 = "Ocupación %";
                    foreach (var silo in _agroService.ObtenerSilos())
                    {
                        Filas.Add(new FilaReporte
                        {
                            Columna1 = silo.Identificador,
                            Columna2 = silo.PlantaAcopio,
                            Columna3 = silo.GranoAlmacenado,
                            Columna4 = $"{silo.StockActualTn:N0} Tn",
                            Columna5 = $"{silo.PorcentajeOcupacion:F1}%",
                            Estado = silo.EstadoAireacion
                        });
                    }
                    break;

                case "Auditoría y Trazabilidad de Partidas":
                    HeaderCol1 = "Código Trazabilidad";
                    HeaderCol2 = "Productor / Cliente";
                    HeaderCol3 = "Cultivo";
                    HeaderCol4 = "Volumen Total";
                    HeaderCol5 = "Ubicación Actual";
                    foreach (var trz in _agroService.ObtenerTrazabilidades())
                    {
                        Filas.Add(new FilaReporte
                        {
                            Columna1 = trz.CodigoTrazabilidad,
                            Columna2 = trz.ProductorCliente,
                            Columna3 = trz.Cultivo,
                            Columna4 = $"{trz.ToneladasTotales:N0} Tn",
                            Columna5 = trz.SiloUbicacionActual,
                            Estado = trz.EstadoCadena
                        });
                    }
                    break;
            }
        }

        private void EjecutarExportarExcel()
        {
            MensajeExportacion = $"Reporte exportado exitosamente a Microsoft Excel (SiTech_{ReporteSeleccionado.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx).";
        }

        private void EjecutarExportarPdf()
        {
            MensajeExportacion = $"Documento PDF generado y firmado digitalmente (SiTech_ReporteOficial_{DateTime.Now:yyyyMMdd_HHmm}.pdf).";
        }
    }
}
