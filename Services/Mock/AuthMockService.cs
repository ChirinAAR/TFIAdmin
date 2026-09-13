using System;
using System.Collections.Generic;
using System.Linq;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;

namespace SiTech.AgroLogistica.Services.Mock
{
    public class AuthMockService : IAuthService
    {
        private readonly List<Usuario> _usuariosDemo = new()
        {
            new Usuario
            {
                Id = 1,
                Username = "admin",
                Password = "123",
                NombreCompleto = "Ing. Lucas Medina",
                Email = "lmedina@sitech.com.ar",
                Rol = "Administrador General",
                AvatarInitials = "LM",
                Activo = true,
                UltimoAcceso = DateTime.Now
            },
            new Usuario
            {
                Id = 2,
                Username = "logistica",
                Password = "123",
                NombreCompleto = "Lic. Sofía Albarracín",
                Email = "salbarracin@sitech.com.ar",
                Rol = "Jefe de Logística y Flota",
                AvatarInitials = "SA",
                Activo = true,
                UltimoAcceso = DateTime.Now.AddMinutes(-20)
            },
            new Usuario
            {
                Id = 3,
                Username = "agronomo",
                Password = "123",
                NombreCompleto = "Ing. Germán Paz",
                Email = "gpaz@sitech.com.ar",
                Rol = "Especialista Agronómico de Campo",
                AvatarInitials = "GP",
                Activo = true,
                UltimoAcceso = DateTime.Now.AddHours(-1)
            },
            new Usuario
            {
                Id = 4,
                Username = "balanza",
                Password = "123",
                NombreCompleto = "Rodrigo Juárez",
                Email = "rjuarez@sitech.com.ar",
                Rol = "Operador de Balanza & Acopio",
                AvatarInitials = "RJ",
                Activo = true,
                UltimoAcceso = DateTime.Now.AddMinutes(-5)
            }
        };

        private Usuario? _usuarioActual;

        public Usuario? UsuarioActual => _usuarioActual;

        public bool IniciarSesion(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var user = _usuariosDemo.FirstOrDefault(u => 
                u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase) && 
                u.Password == password.Trim());

            if (user != null)
            {
                _usuarioActual = user;
                _usuarioActual.UltimoAcceso = DateTime.Now;
                return true;
            }

            return false;
        }

        public bool IniciarSesionRapida(Usuario usuarioDemo)
        {
            if (usuarioDemo == null) return false;
            _usuarioActual = usuarioDemo;
            _usuarioActual.UltimoAcceso = DateTime.Now;
            return true;
        }

        public void CerrarSesion()
        {
            _usuarioActual = null;
        }

        public IReadOnlyList<Usuario> ObtenerUsuariosDemostracion()
        {
            return _usuariosDemo.AsReadOnly();
        }
    }
}
