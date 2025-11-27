using Microsoft.EntityFrameworkCore;

namespace GradeFlow_C_WPF.Data
{
    public class AppDbContext : DbContext
    {
        // New Tables for Saving Data
        public DbSet<UserProfileEntity> UserProfiles { get; set; }
        public DbSet<AcademicPeriodEntity> AcademicPeriods { get; set; }
        public DbSet<CourseEntity> Courses { get; set; }
        public DbSet<GradeCalculatorEntity> GradeCalculators { get; set; }
        public DbSet<GradeItemEntity> GradeItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Creates a new version of the database file
            optionsBuilder.UseSqlite($"Data Source=gradeflow_v2.db");
        }
    }
}