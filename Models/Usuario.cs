using System;

namespace SiTech.AgroLogistica.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operador"; // Administrador, Jefe de Logística, Ingeniero Agrónomo, Operador de Balanza
        public string AvatarInitials { get; set; } = "US";
        public bool Activo { get; set; } = true;
        public DateTime UltimoAcceso { get; set; } = DateTime.Now;
    }
}
