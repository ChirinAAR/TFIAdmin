using System.Collections.Generic;
using SiTech.AgroLogistica.Models;

namespace SiTech.AgroLogistica.Services.Interfaces
{
    public interface IAuthService
    {
        Usuario? UsuarioActual { get; }
        bool IniciarSesion(string username, string password);
        bool IniciarSesionRapida(Usuario usuarioDemo);
        void CerrarSesion();
        IReadOnlyList<Usuario> ObtenerUsuariosDemostracion();
    }
}
