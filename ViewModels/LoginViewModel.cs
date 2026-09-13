using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly Action<Usuario> _onLoginExitoso;

        private string _username = "admin";
        private string _password = "123";
        private string _mensajeError = string.Empty;
        private bool _hayError;

        public LoginViewModel(IAuthService authService, Action<Usuario> onLoginExitoso)
        {
            _authService = authService;
            _onLoginExitoso = onLoginExitoso;

            UsuariosDemo = new ObservableCollection<Usuario>(_authService.ObtenerUsuariosDemostracion());

            IniciarSesionCommand = new RelayCommand(EjecutarIniciarSesion);
            SeleccionarPerfilDemoCommand = new RelayCommand(p => EjecutarPerfilDemo(p as Usuario));
        }

        public ObservableCollection<Usuario> UsuariosDemo { get; }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string MensajeError
        {
            get => _mensajeError;
            set => SetProperty(ref _mensajeError, value);
        }

        public bool HayError
        {
            get => _hayError;
            set => SetProperty(ref _hayError, value);
        }

        public ICommand IniciarSesionCommand { get; }
        public ICommand SeleccionarPerfilDemoCommand { get; }

        private void EjecutarIniciarSesion()
        {
            HayError = false;
            MensajeError = string.Empty;

            if (_authService.IniciarSesion(Username, Password) && _authService.UsuarioActual != null)
            {
                _onLoginExitoso(_authService.UsuarioActual);
            }
            else
            {
                HayError = true;
                MensajeError = "Credenciales incorrectas. Puede usar un perfil demo de acceso rápido.";
            }
        }

        private void EjecutarPerfilDemo(Usuario? usuario)
        {
            if (usuario == null) return;
            if (_authService.IniciarSesionRapida(usuario) && _authService.UsuarioActual != null)
            {
                _onLoginExitoso(_authService.UsuarioActual);
            }
        }
    }
}
