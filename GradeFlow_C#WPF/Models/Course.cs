using System;

namespace GradeFlow_C_WPF.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Credits { get; set; }
        public string GradeLetter { get; set; } = string.Empty;

        public double GradePoints
        {
            get
            {
                // CHANGED: Matches the new dropdown string formats
                return GradeLetter switch
                {
                    "A+ (90-100)" => 4.0,
                    "A (85-89)" => 4.0,
                    "A- (80-84)" => 3.7,
                    "B+ (77-79)" => 3.3,
                    "B (73-76)" => 3.0,
                    "B- (70-72)" => 2.7,
                    "C+ (67-69)" => 2.3,
                    "C (63-66)" => 2.0,
                    "C- (60-62)" => 1.7,
                    "D+ (57-59)" => 1.3,
                    "D (53-56)" => 1.0,
                    "D- (50-52)" => 0.7,
                    "F (0-49)" => 0.0,
                    _ => 0.0
                };
            }
        }
    }
}