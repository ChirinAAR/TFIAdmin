using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class TrazabilidadViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private string _textoBusqueda = string.Empty;
        private TrazabilidadItem? _partidaSeleccionada;
        private string _mensajeCertificado = string.Empty;

        public TrazabilidadViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Partidas = new ObservableCollection<TrazabilidadItem>();

            BuscarCommand = new RelayCommand(EjecutarBuscar);
            GenerarCertificadoCommand = new RelayCommand(EjecutarGenerarCertificado, () => PartidaSeleccionada != null);

            CargarPartidas();
        }

        public ObservableCollection<TrazabilidadItem> Partidas { get; }

        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set => SetProperty(ref _textoBusqueda, value);
        }

        public TrazabilidadItem? PartidaSeleccionada
        {
            get => _partidaSeleccionada;
            set
            {
                if (SetProperty(ref _partidaSeleccionada, value))
                    MensajeCertificado = string.Empty;
            }
        }

        public string MensajeCertificado
        {
            get => _mensajeCertificado;
            set => SetProperty(ref _mensajeCertificado, value);
        }

        public ICommand BuscarCommand { get; }
        public ICommand GenerarCertificadoCommand { get; }

        public void CargarPartidas()
        {
            Partidas.Clear();
            foreach (var item in _agroService.ObtenerTrazabilidades())
            {
                Partidas.Add(item);
            }
            if (PartidaSeleccionada == null && Partidas.Any())
                PartidaSeleccionada = Partidas.First();
        }

        private void EjecutarBuscar()
        {
            var resultado = _agroService.BuscarTrazabilidad(TextoBusqueda);
            if (resultado != null)
            {
                PartidaSeleccionada = resultado;
            }
        }

        private void EjecutarGenerarCertificado()
        {
            if (PartidaSeleccionada != null)
            {
                string hash = Guid.NewGuid().ToString("N")[..12].ToUpper();
                MensajeCertificado = $"Certificado de Origen y Trazabilidad emitido con éxito. Hash de Inocuidad: SI-TECH-NOA-{hash}. Válido para Senasa y Aduana.";
            }
        }
    }
}
