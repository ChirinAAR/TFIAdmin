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

            // Login rápido con perfil demo
            var perfiles = auth.ObtenerUsuariosDemostracion();
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
        public void MainViewModel_FlujoNavegacionYVistas()
        {
            var auth = new AuthMockService();
            var dataService = new AgroMockDataService();
            var mainVM = new MainViewModel(auth, dataService);

            // Estado inicial: No autenticado
            Assert.IsFalse(mainVM.EstaAutenticado);
            Assert.IsInstanceOfType(mainVM.CurrentViewModel, typeof(LoginViewModel));

            // Simular Login rápido
            var loginVM = (LoginViewModel)mainVM.CurrentViewModel!;
            var adminUser = auth.ObtenerUsuariosDemostracion().First(u => u.Username == "admin");
            loginVM.SeleccionarPerfilDemoCommand.Execute(adminUser);

            // Ahora debe estar autenticado y en Dashboard
            Assert.IsTrue(mainVM.EstaAutenticado);
            Assert.IsNotNull(mainVM.UsuarioActual);
            Assert.AreEqual("Dashboard", mainVM.MenuActivo);
            Assert.IsInstanceOfType(mainVM.CurrentViewModel, typeof(DashboardViewModel));

            // Probar navegación por todos los menús solicitados
            string[] menus = new[]
            {
                "Cultivos", "Cosecha", "Transporte", "Acopio",
                "Clima", "Trazabilidad", "Clientes", "Campanas",
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
    }
}
