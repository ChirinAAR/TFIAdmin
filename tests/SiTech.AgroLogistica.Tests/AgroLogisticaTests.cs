using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Mock;
using SiTech.AgroLogistica.ViewModels;

namespace SiTech.AgroLogistica.Tests
{
    [TestClass]
    public class AgroLogisticaTests
    {
        [TestMethod]
        public void AuthMockService_ValidarCredencialesYPerfilesDemo()
        {
            var auth = new AuthMockService();

            // Login fallido
            bool loginInvalido = auth.IniciarSesion("usuario_inexistente", "clave_erronea");
            Assert.IsFalse(loginInvalido);
            Assert.IsNull(auth.UsuarioActual);

            // Login exitoso admin
            bool loginExitoso = auth.IniciarSesion("admin", "123");
            Assert.IsTrue(loginExitoso);
            Assert.IsNotNull(auth.UsuarioActual);
            Assert.AreEqual("Administrador General", auth.UsuarioActual.Rol);

            // Cierre de sesión
            auth.CerrarSesion();
            Assert.IsNull(auth.UsuarioActual);

            // Login con usuario del sistema
            var perfiles = auth.ObtenerUsuariosSistema();
            Assert.IsTrue(perfiles.Count >= 4);
            var agronomo = perfiles.First(p => p.Username == "agronomo");
            auth.IniciarSesionRapida(agronomo);
            Assert.IsNotNull(auth.UsuarioActual);
            Assert.AreEqual("Especialista Agronómico de Campo", auth.UsuarioActual.Rol);
        }

        [TestMethod]
        public void AgroMockDataService_OperacionesDeCultivosYCampanas()
        {
            var dataService = new AgroMockDataService();

            var lotesIniciales = dataService.ObtenerLotes();
            Assert.IsTrue(lotesIniciales.Count >= 5);

            // Agregar nuevo lote
            var nuevoLote = new CultivoLote
            {
                Nombre = "Lote Test Bella Vista",
                FincaEstablecimiento = "Finca Bella Vista",
                ZonaUbicacion = "Famaillá, Tucumán",
                Hectareas = 120,
                TipoCultivo = "Soja",
                VariedadHibrido = "DM 46i20",
                EstadoFenologico = "R2 - Floración plena",
                HumedadSueloPorcentaje = 64.0,
                IndiceVerdeNDVI = 0.79,
                EstadoSanitario = "Óptimo",
                FechaSiembra = DateTime.Now.AddMonths(-2),
                FechaEstimadaCosecha = DateTime.Now.AddMonths(2),
                RindeEsperadoTnHa = 3.6
            };

            dataService.AgregarLote(nuevoLote);
            var lotesActualizados = dataService.ObtenerLotes();
            Assert.AreEqual(lotesIniciales.Count, lotesActualizados.Count);
            Assert.IsTrue(lotesActualizados.Any(l => l.Nombre == "Lote Test Bella Vista"));

            // Búsqueda por código
            var encontrado = dataService.ObtenerLotePorCodigo("Bella Vista");
            Assert.IsNotNull(encontrado);
            Assert.AreEqual("Lote Test Bella Vista", encontrado.Nombre);
        }

        [TestMethod]
        public void AgroMockDataService_CosechaAvanceYLogistica()
        {
            var dataService = new AgroMockDataService();

            // Cosecha
            var labores = dataService.ObtenerLaboresCosecha();
            var labor1 = labores.First();
            double tnIniciales = labor1.ToneladasRecolectadas;
            dataService.RegistrarAvanceCosecha(labor1.Id, 50.0, 75.0);
            Assert.AreEqual(tnIniciales + 50.0, labor1.ToneladasRecolectadas);
            Assert.AreEqual(75.0, labor1.PorcentajeAvance);

            // Transporte
            var viajes = dataService.ObtenerViajesTransporte();
            Assert.IsTrue(viajes.Count >= 3);
            var viaje1 = viajes.First();
            dataService.ActualizarEstadoViaje(viaje1.Id, "Descargado", 100);
            Assert.AreEqual("Descargado", viaje1.Estado);
            Assert.AreEqual(100.0, viaje1.ProgresoRuta);

            // Silos
            var silos = dataService.ObtenerSilos();
            var silo1 = silos.First();
            double stockInicial = silo1.StockActualTn;
            dataService.ActualizarStockSilo(silo1.Id, 30.0);
            Assert.AreEqual(stockInicial + 30.0, silo1.StockActualTn);
        }

        [TestMethod]
        public void AgroMockDataService_AlertasClimaYTrazabilidad()
        {
            var dataService = new AgroMockDataService();

            var estaciones = dataService.ObtenerEstacionesClima();
            Assert.IsTrue(estaciones.Count >= 4);

            var alertas = dataService.ObtenerAlertas();
            int cantidadInicial = alertas.Count;

            // Agregar Alerta
            var nuevaAlerta = new AlertaMeteorologica
            {
                Titulo = "Alerta Helada Test",
                TipoAlerta = "Helada Tardía",
                NivelSeveridad = "Crítica",
                ZonaAfectada = "Valles Calchaquíes",
                Descripcion = "Descenso brusco térmico",
                RecomendacionOperativa = "Activar riego por aspersión antihelada"
            };
            dataService.AgregarAlerta(nuevaAlerta);
            Assert.AreEqual(cantidadInicial + 1, dataService.ObtenerAlertas().Count);

            // Resolver Alerta
            dataService.ResolverAlerta(nuevaAlerta.Id);
            Assert.IsFalse(nuevaAlerta.Activa);

            // Trazabilidad
            var trz = dataService.BuscarTrazabilidad("TRZ-2026-SOJ-0914");
            Assert.IsNotNull(trz);
            Assert.IsTrue(trz.HistorialHitos.Count >= 5);
        }

        [TestMethod]
        public void DashboardViewModel_AlertasMultiareaYFiltros()
        {
            var dataService = new AgroMockDataService();
            string? moduloNavegado = null;
            var dashboardVM = new DashboardViewModel(dataService, modulo => moduloNavegado = modulo);

            // Verificar que hay alertas de múltiples áreas activas
            var alertas = dashboardVM.AlertasUrgentes;
            Assert.IsTrue(alertas.Count >= 5, "Debe contener alertas activas de múltiples áreas");

            var modulosPresentes = alertas.Select(a => a.ModuloOrigen).Distinct().ToList();
            CollectionAssert.Contains(modulosPresentes, "Cultivos");
            CollectionAssert.Contains(modulosPresentes, "Cosecha");
            CollectionAssert.Contains(modulosPresentes, "Transporte");
            CollectionAssert.Contains(modulosPresentes, "Acopio");
            CollectionAssert.Contains(modulosPresentes, "Clima");

            // Probar filtrado por área
            dashboardVM.FiltrarAreaCommand.Execute("Acopio");
            Assert.AreEqual("Acopio", dashboardVM.FiltroArea);
            Assert.IsTrue(dashboardVM.AlertasUrgentes.All(a => a.ModuloOrigen == "Acopio"));
            Assert.IsTrue(dashboardVM.AlertasUrgentes.Count >= 1);

            // Probar filtrado por Cultivos
            dashboardVM.FiltrarAreaCommand.Execute("Cultivos");
            Assert.AreEqual("Cultivos", dashboardVM.FiltroArea);
            Assert.IsTrue(dashboardVM.AlertasUrgentes.All(a => a.ModuloOrigen == "Cultivos"));

            // Volver a "Todas"
            dashboardVM.FiltrarAreaCommand.Execute("Todas");
            Assert.AreEqual("Todas", dashboardVM.FiltroArea);
            Assert.IsTrue(dashboardVM.AlertasUrgentes.Count >= 5);

            // Probar comando de resolver alerta directamente en Dashboard
            int totalAntes = dashboardVM.AlertasUrgentes.Count;
            var primeraAlerta = dashboardVM.AlertasUrgentes.First();
            dashboardVM.ResolverAlertaCommand.Execute(primeraAlerta);
            Assert.AreEqual(totalAntes - 1, dashboardVM.AlertasUrgentes.Count);

            // Probar comando de navegación a módulo de alerta
            var alertaAcopio = dashboardVM.AlertasUrgentes.First(a => a.ModuloOrigen == "Acopio");
            dashboardVM.NavegarAModuloAlertaCommand.Execute(alertaAcopio);
            Assert.AreEqual("Acopio", moduloNavegado);

            var alertaCultivos = dashboardVM.AlertasUrgentes.First(a => a.ModuloOrigen == "Cultivos");
            dashboardVM.NavegarAModuloAlertaCommand.Execute(alertaCultivos);
            Assert.AreEqual("Cultivos", moduloNavegado);
        }

        [TestMethod]
        public void MainViewModel_FlujoNavegacionYVistas()
        {
            var auth = new AuthMockService();
            var dataService = new AgroMockDataService();
            var mainVM = new MainViewModel(auth, dataService);

            // Estado inicial: No autenticado
            Assert.IsFalse(mainVM.EstaAutenticado);
            Assert.IsInstanceOfType(mainVM.CurrentViewModel, typeof(LoginViewModel));

            // Simular Login
            var loginVM = (LoginViewModel)mainVM.CurrentViewModel!;
            loginVM.Username = "admin";
            loginVM.Password = "123";
            loginVM.IniciarSesionCommand.Execute(null);

            // Ahora debe estar autenticado y en Dashboard
            Assert.IsTrue(mainVM.EstaAutenticado);
            Assert.IsNotNull(mainVM.UsuarioActual);
            Assert.AreEqual("Dashboard", mainVM.MenuActivo);
            Assert.IsInstanceOfType(mainVM.CurrentViewModel, typeof(DashboardViewModel));

            // Probar navegación por todos los menús solicitados
            string[] menus = new[]
            {
                "Cultivos", "Cosecha", "Transporte", "Acopio",
                "Clima", "Clientes", "Campanas",
                "Empleados", "Reportes", "Configuracion", "Dashboard"
            };

            foreach (var menu in menus)
            {
                mainVM.NavegarCommand.Execute(menu);
                Assert.AreEqual(menu, mainVM.MenuActivo);
                Assert.IsNotNull(mainVM.CurrentViewModel);
            }

            // Probar Logout
            mainVM.CerrarSesionCommand.Execute(null);
            Assert.IsFalse(mainVM.EstaAutenticado);
            Assert.IsNull(mainVM.UsuarioActual);
            Assert.IsInstanceOfType(mainVM.CurrentViewModel, typeof(LoginViewModel));
        }

        [TestMethod]
        public void AcopioView_InstanciarSinExcepcion()
        {
            Exception? exception = null;
            var thread = new System.Threading.Thread(() =>
            {
                try
                {
                    if (System.Windows.Application.Current == null)
                    {
                        var app = new System.Windows.Application();
                    }

                    var appUri = new Uri("pack://application:,,,/SiTech.AgroLogistica;component/App.xaml", UriKind.Absolute);
                    // To avoid creating multiple Application instances, just load the merged dictionaries and templates
                    var colors = new System.Windows.ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/SiTech.AgroLogistica;component/Styles/Colors.xaml", UriKind.Absolute)
                    };
                    var controls = new System.Windows.ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/SiTech.AgroLogistica;component/Styles/Controls.xaml", UriKind.Absolute)
                    };
                    System.Windows.Application.Current!.Resources.MergedDictionaries.Add(colors);
                    System.Windows.Application.Current.Resources.MergedDictionaries.Add(controls);
                    System.Windows.Application.Current.Resources.Add("BooleanToVisibilityConverter", new System.Windows.Controls.BooleanToVisibilityConverter());
                    System.Windows.Application.Current.Resources.Add("InverseBooleanToVisibilityConverter", new SiTech.AgroLogistica.ViewModels.Common.InverseBooleanToVisibilityConverter());

                    var acopioTemplate = new System.Windows.DataTemplate(typeof(AcopioViewModel));
                    var factory = new System.Windows.FrameworkElementFactory(typeof(SiTech.AgroLogistica.Views.AcopioView));
                    acopioTemplate.VisualTree = factory;
                    System.Windows.Application.Current.Resources.Add(new System.Windows.DataTemplateKey(typeof(AcopioViewModel)), acopioTemplate);

                    var authService = new AuthMockService();
                    var agroService = new AgroMockDataService();
                    var mainVM = new MainViewModel(authService, agroService);

                    authService.IniciarSesion("admin", "123");
                    mainVM.EstaAutenticado = true;
                    mainVM.UsuarioActual = authService.UsuarioActual;

                    var window = new MainWindow
                    {
                        DataContext = mainVM
                    };

                    // Probar navegación a Acopio
                    mainVM.NavegarA("Acopio");
                    window.Measure(new System.Windows.Size(1200, 800));
                    window.Arrange(new System.Windows.Rect(0, 0, 1200, 800));
                    window.UpdateLayout();

                    // Probar todas las vistas
                    string[] vistas = { "Dashboard", "Cultivos", "Cosecha", "Transporte", "Acopio", "Clima", "Clientes", "Campanas", "Empleados", "Reportes", "Configuracion" };
                    foreach (var v in vistas)
                    {
                        mainVM.NavegarA(v);
                        window.Measure(new System.Windows.Size(1200, 800));
                        window.Arrange(new System.Windows.Rect(0, 0, 1200, 800));
                        window.UpdateLayout();
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
            {
                var sb = new System.Text.StringBuilder();
                var cur = exception;
                while (cur != null)
                {
                    sb.AppendLine($"[{cur.GetType().Name}] {cur.Message}\n{cur.StackTrace}\n---");
                    cur = cur.InnerException;
                }
                throw new Exception($"EXCEPCIÓN DETALLADA:\n{sb.ToString()}");
            }
        }

        [TestMethod]
        public void ClimaAlertasViewModel_TelemetriaYPronosticoExtendido()
        {
            var dataService = new AgroMockDataService();
            var climaVM = new ClimaAlertasViewModel(dataService);

            // Verificar estaciones cargadas
            Assert.IsTrue(climaVM.Estaciones.Count >= 4);
            Assert.IsNotNull(climaVM.EstacionSeleccionada);
            Assert.IsTrue(climaVM.PronosticoExtendido.Count >= 5);

            // Verificar propiedades agrometeorológicas
            var estActiva = climaVM.EstacionSeleccionada;
            Assert.IsTrue(estActiva.DeltaTC > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(estActiva.CondicionPulverizacion));
            Assert.IsFalse(string.IsNullOrWhiteSpace(estActiva.DiagnosticoVentana));

            // Probar cambio de estación
            var segundaEstacion = climaVM.Estaciones.Skip(1).First();
            climaVM.SeleccionarEstacionCommand.Execute(segundaEstacion);
            Assert.AreEqual(segundaEstacion.Id, climaVM.EstacionSeleccionada.Id);
            Assert.AreEqual(segundaEstacion.NombreEstacion, climaVM.EstacionSeleccionada.NombreEstacion);
            Assert.IsTrue(climaVM.PronosticoExtendido.Count >= 5);
        }

        [TestMethod]
        public void ConfiguracionViewModel_ParametrosAgrologisticaYGuardado()
        {
            var auth = new AuthMockService();
            var configVM = new ConfiguracionViewModel(auth);

            // Verificar catálogos y colecciones
            Assert.IsTrue(configVM.ListaPuertosBalanza.Count >= 3);
            Assert.IsTrue(configVM.ListaModelosBalanza.Count >= 3);
            Assert.IsTrue(configVM.ListaAmbientesAfip.Count >= 2);
            Assert.IsTrue(configVM.ListaProveedoresGps.Count >= 3);
            Assert.IsTrue(configVM.ListaConstelaciones.Count >= 3);
            Assert.IsTrue(configVM.UsuariosSistema.Count >= 4);

            // Probar prueba de balanza
            configVM.ProbarBalanzaCommand.Execute(null);
            Assert.IsTrue(configVM.MensajePruebaBalanza.Contains("Balanza conectada"));

            // Probar comando de guardar
            configVM.GuardarConfiguracionCommand.Execute(null);
            Assert.IsTrue(configVM.MensajeGuardado.Contains("Configuración guardada"));

            // Modificar valores y restablecer
            configVM.UmbralTemperaturaSiloC = 35.0;
            configVM.RestablecerValoresCommand.Execute(null);
            Assert.AreEqual(26.0, configVM.UmbralTemperaturaSiloC);
            Assert.IsTrue(configVM.MensajeGuardado.Contains("restablecidos"));
        }
    }
}
