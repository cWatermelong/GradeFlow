using System.Collections.Generic;

namespace GradeFlow_C_WPF.Data
{
    // Table: Stores Current CGPA/Credits input
    public class UserProfileEntity
    {
        public int Id { get; set; }
        public string CurrentCGPA { get; set; } = "";
        public string CurrentCredits { get; set; } = "";
    }

    // Table: Stores Semesters (e.g., "Fall 2024")
    public class AcademicPeriodEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<CourseEntity> Courses { get; set; } = new();
    }

    // Table: Stores Courses inside Semesters
    public class CourseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double Credits { get; set; }
        public string Grade { get; set; } = "";

        public int AcademicPeriodId { get; set; }
        public AcademicPeriodEntity AcademicPeriod { get; set; }
    }

    // Table: Stores the "Course Grade" Calculator Cards
    public class GradeCalculatorEntity
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = "";
        public List<GradeItemEntity> Items { get; set; } = new();
    }

    // Table: Stores rows inside the Calculator Cards (e.g. "Midterm", 50/70)
    public class GradeItemEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string GradeInput { get; set; } = "";
        public string Weight { get; set; } = "";

        public int GradeCalculatorId { get; set; }
        public GradeCalculatorEntity GradeCalculator { get; set; }
    }
}