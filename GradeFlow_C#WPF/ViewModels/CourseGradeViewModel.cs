using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GradeFlow_C_WPF.Models;
using System;
using System.Collections.ObjectModel;

namespace GradeFlow_C_WPF.ViewModels
{
    public partial class CourseGradeViewModel : ObservableObject
    {
        // Callback to send data back to MainViewModel
        private readonly Action<string, string, string> _addToSemesterCallback;

        [ObservableProperty]
        private string courseName = "New Course";

        // NEW: Added Credits so it can be sent to the scenario planner
        [ObservableProperty]
        private string credits = "0.5";

        [ObservableProperty]
        private string finalGradeText = "Final Grade: -";

        [ObservableProperty]
        private string letterGradeText = "";

        [ObservableProperty]
        private string gradeColor = "#10B981";

        public ObservableCollection<GradeItem> GradeItems { get; } = new();

        // Constructor now requires the callback
        public CourseGradeViewModel(Action<string, string, string> addToSemesterCallback = null)
        {
            _addToSemesterCallback = addToSemesterCallback;

            GradeItems.Add(new GradeItem { Name = "Assignment", Weight = "20" });
            GradeItems.Add(new GradeItem { Name = "Midterm", Weight = "30" });
            GradeItems.Add(new GradeItem { Name = "Final", Weight = "50" });
        }

        [RelayCommand]
        public void AddRow() => GradeItems.Add(new GradeItem { Name = "New Item" });

        [RelayCommand]
        public void RemoveRow(GradeItem item) => GradeItems.Remove(item);

        [RelayCommand]
        public void Calculate()
        {
            double totalWeightedScore = 0;
            double totalWeight = 0;

            foreach (var item in GradeItems)
            {
                if (string.IsNullOrWhiteSpace(item.GradeInput) || string.IsNullOrWhiteSpace(item.Weight))
                    continue;

                double score = ParseGradeInput(item.GradeInput);

                if (double.TryParse(item.Weight, out double weight))
                {
                    totalWeightedScore += (score / 100) * weight;
                    totalWeight += weight;
                }
            }

            string letter = GetLetterGrade(totalWeightedScore);

            FinalGradeText = $"Final Grade: {totalWeightedScore:F2}%";
            LetterGradeText = $"{letter}";
            GradeColor = letter.StartsWith("F") ? "#EF4444" : "#10B981";
        }

        [RelayCommand]
        public void Clear()
        {
            foreach (var item in GradeItems) item.GradeInput = "";
            FinalGradeText = "Final Grade: -";
            LetterGradeText = "";
            GradeColor = "#10B981";
        }

        // NEW: Command to send this result to the Main Tab
        [RelayCommand]
        public void AddToSemester()
        {
            // Ensure we have a grade to send
            if (string.IsNullOrEmpty(LetterGradeText))
                Calculate();

            if (!string.IsNullOrEmpty(LetterGradeText))
            {
                _addToSemesterCallback?.Invoke(CourseName, Credits, LetterGradeText);
            }
        }

        private double ParseGradeInput(string input)
        {
            try
            {
                if (input.Contains("/"))
                {
                    var parts = input.Split('/');
                    if (parts.Length == 2 && double.TryParse(parts[0], out double n) && double.TryParse(parts[1], out double d) && d != 0)
                        return (n / d) * 100;
                }
                else if (double.TryParse(input, out double val)) return val;
            }
            catch { }
            return 0;
        }

        // Updated logic to match the Course.cs exact string format
        private string GetLetterGrade(double score)
        {
            if (score >= 90) return "A+ (90-100)";
            if (score >= 85) return "A (85-89)";
            if (score >= 80) return "A- (80-84)";
            if (score >= 77) return "B+ (77-79)";
            if (score >= 73) return "B (73-76)";
            if (score >= 70) return "B- (70-72)";
            if (score >= 67) return "C+ (67-69)";
            if (score >= 63) return "C (63-66)";
            if (score >= 60) return "C- (60-62)";
            if (score >= 57) return "D+ (57-59)";
            if (score >= 53) return "D (53-56)";
            if (score >= 50) return "D- (50-52)";
            return "F (0-49)";
        }
    }
}