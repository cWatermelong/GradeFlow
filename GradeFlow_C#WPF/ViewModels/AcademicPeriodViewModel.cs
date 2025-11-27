using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GradeFlow_C_WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GradeFlow_C_WPF.ViewModels
{
    public partial class AcademicPeriodViewModel : ObservableObject
    {
        private readonly Action _recalculateParentCallback;

        [ObservableProperty] private string periodName = "New Semester";
        [ObservableProperty] private string newCourseName = "";
        [ObservableProperty] private string newCourseCredits = "0.5";
        [ObservableProperty] private string newCourseGrade = "";
        [ObservableProperty] private string sgpa = "0.00";

        public ObservableCollection<Course> Courses { get; } = new();

        public ObservableCollection<string> GradeOptions { get; } = new()
        {
            "A+ (90-100)", "A (85-89)", "A- (80-84)", "B+ (77-79)", "B (73-76)", "B- (70-72)",
            "C+ (67-69)", "C (63-66)", "C- (60-62)", "D+ (57-59)", "D (53-56)", "D- (50-52)",
            "F (0-49)"
        };

        public AcademicPeriodViewModel(Action recalculateCallback)
        {
            _recalculateParentCallback = recalculateCallback;
        }

        // Helper called by MainViewModel after loading saved data
        public void ForceRecalculate() => RecalculateSGPA();

        [RelayCommand]
        public void AddCourse()
        {
            if (string.IsNullOrWhiteSpace(NewCourseName) || string.IsNullOrWhiteSpace(NewCourseGrade) || !double.TryParse(NewCourseCredits, out double credits)) return;

            Courses.Add(new Course { Name = NewCourseName, Credits = credits, GradeLetter = NewCourseGrade });
            NewCourseName = ""; NewCourseCredits = "0.5"; NewCourseGrade = null;
            RecalculateSGPA();
            _recalculateParentCallback?.Invoke();
        }

        [RelayCommand]
        public void DeleteCourse(Course course)
        {
            if (Courses.Contains(course))
            {
                Courses.Remove(course);
                RecalculateSGPA();
                _recalculateParentCallback?.Invoke();
            }
        }

        private void RecalculateSGPA()
        {
            if (Courses.Count == 0) { Sgpa = "0.00"; return; }
            double totalPoints = Courses.Sum(c => c.GradePoints * c.Credits);
            double totalCredits = Courses.Sum(c => c.Credits);
            Sgpa = totalCredits > 0 ? (totalPoints / totalCredits).ToString("F2") : "0.00";
        }
    }
}