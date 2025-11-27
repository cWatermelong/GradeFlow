using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GradeFlow_C_WPF.Data;
using GradeFlow_C_WPF.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace GradeFlow_C_WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly AppDbContext _context;

        [ObservableProperty] private string currentCGPAInput = "";
        [ObservableProperty] private string currentCreditsInput = "";
        [ObservableProperty] private string projectedCGPA = "0.00";
        [ObservableProperty] private double projectedTotalCredits = 0;

        public ObservableCollection<AcademicPeriodViewModel> AcademicPeriods { get; } = new();

        [ObservableProperty] private int currentViewIndex = 0;

        public ObservableCollection<CourseGradeViewModel> GradeCalculators { get; } = new();

        public MainViewModel()
        {
            _context = new AppDbContext();
            _context.Database.EnsureCreated();
            LoadData();
        }

        [RelayCommand] public void NavigateToDashboard() => CurrentViewIndex = 0;
        [RelayCommand] public void NavigateToGrades() => CurrentViewIndex = 1;

        // --- NEW: Integration Logic ---
        public void AddCourseFromCalculator(string name, string credits, string grade)
        {
            // 1. Ensure there is at least one semester to add to
            if (AcademicPeriods.Count == 0) AddAcademicPeriod();

            // 2. Get the last semester (usually the one the user is working on)
            var targetSemester = AcademicPeriods.Last();

            // 3. Parse Credits
            if (!double.TryParse(credits, out double creditVal)) creditVal = 0.5;

            // 4. Add to the semester
            targetSemester.Courses.Add(new Course
            {
                Name = name,
                Credits = creditVal,
                GradeLetter = grade
            });

            // 5. Update Calculations
            targetSemester.ForceRecalculate();
            RecalculateProjectedCGPA();

            // 6. Switch view to show the user the result
            CurrentViewIndex = 0;
        }

        // --- PERSISTENCE ---
        [RelayCommand]
        public void SaveData()
        {
            _context.UserProfiles.RemoveRange(_context.UserProfiles);
            _context.AcademicPeriods.RemoveRange(_context.AcademicPeriods);
            _context.GradeCalculators.RemoveRange(_context.GradeCalculators);
            _context.SaveChanges();

            _context.UserProfiles.Add(new UserProfileEntity { CurrentCGPA = CurrentCGPAInput, CurrentCredits = CurrentCreditsInput });

            foreach (var periodVM in AcademicPeriods)
            {
                var periodEntity = new AcademicPeriodEntity { Name = periodVM.PeriodName };
                foreach (var courseVM in periodVM.Courses)
                {
                    periodEntity.Courses.Add(new CourseEntity { Name = courseVM.Name, Credits = courseVM.Credits, Grade = courseVM.GradeLetter });
                }
                _context.AcademicPeriods.Add(periodEntity);
            }

            foreach (var calcVM in GradeCalculators)
            {
                var calcEntity = new GradeCalculatorEntity { CourseName = calcVM.CourseName };
                foreach (var itemVM in calcVM.GradeItems)
                {
                    calcEntity.Items.Add(new GradeItemEntity { Name = itemVM.Name, GradeInput = itemVM.GradeInput, Weight = itemVM.Weight });
                }
                _context.GradeCalculators.Add(calcEntity);
            }

            _context.SaveChanges();
            MessageBox.Show("Data Saved Successfully!", "GradeFlow", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadData()
        {
            var profile = _context.UserProfiles.FirstOrDefault();
            if (profile != null) { CurrentCGPAInput = profile.CurrentCGPA; CurrentCreditsInput = profile.CurrentCredits; }

            var periods = _context.AcademicPeriods.Include(p => p.Courses).ToList();
            AcademicPeriods.Clear();
            foreach (var p in periods)
            {
                var vm = new AcademicPeriodViewModel(RecalculateProjectedCGPA) { PeriodName = p.Name };
                foreach (var c in p.Courses) vm.Courses.Add(new Course { Name = c.Name, Credits = c.Credits, GradeLetter = c.Grade });
                vm.ForceRecalculate();
                AcademicPeriods.Add(vm);
            }
            if (AcademicPeriods.Count == 0) AddAcademicPeriod();

            var calculators = _context.GradeCalculators.Include(g => g.Items).ToList();
            GradeCalculators.Clear();
            foreach (var c in calculators)
            {
                // Inject the callback here too
                var vm = new CourseGradeViewModel(AddCourseFromCalculator) { CourseName = c.CourseName };
                vm.GradeItems.Clear();
                foreach (var i in c.Items) vm.GradeItems.Add(new GradeItem { Name = i.Name, GradeInput = i.GradeInput, Weight = i.Weight });
                vm.Calculate();
                GradeCalculators.Add(vm);
            }
            RecalculateProjectedCGPA();
        }

        [RelayCommand] public void AddAcademicPeriod() => AcademicPeriods.Add(new AcademicPeriodViewModel(RecalculateProjectedCGPA));
        [RelayCommand] public void RemoveAcademicPeriod(AcademicPeriodViewModel period) { AcademicPeriods.Remove(period); RecalculateProjectedCGPA(); }

        [RelayCommand]
        public void RecalculateProjectedCGPA()
        {
            double.TryParse(CurrentCGPAInput, out double startGpa);
            double.TryParse(CurrentCreditsInput, out double startCredits);
            double totalPoints = startGpa * startCredits;
            double totalCredits = startCredits;

            foreach (var period in AcademicPeriods)
            {
                foreach (var course in period.Courses)
                {
                    totalPoints += (course.GradePoints * course.Credits);
                    totalCredits += course.Credits;
                }
            }
            ProjectedTotalCredits = totalCredits;
            ProjectedCGPA = totalCredits > 0 ? (totalPoints / totalCredits).ToString("F2") : "0.00";
        }

        // Pass the callback when adding new cards manually
        [RelayCommand] public void AddCalculatorCard() => GradeCalculators.Add(new CourseGradeViewModel(AddCourseFromCalculator));
        [RelayCommand] public void RemoveCalculatorCard(CourseGradeViewModel card) => GradeCalculators.Remove(card);
    }
}