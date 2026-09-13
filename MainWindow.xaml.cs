using System.Windows;
using SiTech.AgroLogistica.ViewModels;

namespace SiTech.AgroLogistica
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}