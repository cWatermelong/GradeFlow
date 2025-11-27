using CommunityToolkit.Mvvm.ComponentModel;

namespace GradeFlow_C_WPF.Models
{
    public partial class GradeItem : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string gradeInput; // User types "50/70" or "85" here

        [ObservableProperty]
        private string weight;     // User types "20" for 20%
    }
}