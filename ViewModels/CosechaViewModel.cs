using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SiTech.AgroLogistica.Models;
using SiTech.AgroLogistica.Services.Interfaces;
using SiTech.AgroLogistica.ViewModels.Common;

namespace SiTech.AgroLogistica.ViewModels
{
    public class CosechaViewModel : ObservableObject
    {
        private readonly IAgroDataService _agroService;
        private CosechaLabor? _laborSeleccionada;
        private bool _mostrarDialogoAvance;
        private double _toneladasAvance = 35.0;

        public CosechaViewModel(IAgroDataService agroService)
        {
            _agroService = agroService;
            Labores = new ObservableCollection<CosechaLabor>();

            RegistrarAvanceCommand = new RelayCommand(AbrirDialogoAvance, () => LaborSeleccionada != null);
            ConfirmarAvanceCommand = new RelayCommand(EjecutarConfirmarAvance);
            CancelarAvanceCommand = new RelayCommand(() => MostrarDialogoAvance = false);

            CargarLabores();
        }

        public ObservableCollection<CosechaLabor> Labores { get; }

        public CosechaLabor? LaborSeleccionada
        {
            get => _laborSeleccionada;
            set => SetProperty(ref _laborSeleccionada, value);
        }

        public bool MostrarDialogoAvance
        {
            get => _mostrarDialogoAvance;
            set => SetProperty(ref _mostrarDialogoAvance, value);
        }

        public double ToneladasAvance
        {
            get => _toneladasAvance;
            set => SetProperty(ref _toneladasAvance, value);
        }

        public ICommand RegistrarAvanceCommand { get; }
        public ICommand ConfirmarAvanceCommand { get; }
        public ICommand CancelarAvanceCommand { get; }

        public void CargarLabores()
        {
            Labores.Clear();
            foreach (var labor in _agroService.ObtenerLaboresCosecha())
            {
                Labores.Add(labor);
            }
            if (LaborSeleccionada == null && Labores.Any())
                LaborSeleccionada = Labores.First();
        }

        private void AbrirDialogoAvance()
        {
            if (LaborSeleccionada != null)
            {
                MostrarDialogoAvance = true;
            }
        }

        private void EjecutarConfirmarAvance()
        {
            if (LaborSeleccionada != null && ToneladasAvance > 0)
            {
                double total = LaborSeleccionada.ToneladasRecolectadas + ToneladasAvance;
                double nuevoAvance = LaborSeleccionada.ToneladasEstimadas > 0
                    ? Math.Min(100, (total / LaborSeleccionada.ToneladasEstimadas) * 100)
                    : 100;

                _agroService.RegistrarAvanceCosecha(LaborSeleccionada.Id, ToneladasAvance, nuevoAvance);
                CargarLabores();
                LaborSeleccionada = Labores.FirstOrDefault(l => l.Id == LaborSeleccionada.Id);
                MostrarDialogoAvance = false;
            }
        }
    }
}
