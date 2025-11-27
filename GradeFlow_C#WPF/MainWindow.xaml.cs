using System.Windows;
using GradeFlow_C_WPF.ViewModels;

namespace GradeFlow_C_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Wire up the ViewModel
            DataContext = new MainViewModel();
        }
    }
}