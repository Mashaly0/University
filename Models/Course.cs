using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace University.Models
    {
        public class Course
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            public string Name { get; set; } = string.Empty;
            public string Name1 { get; set; } = string.Empty;
            public bool IsAvailable { get; set; }


        // 🔹 مسار الملف (PDF / Excel / Word)
        [Display(Name = "الملف المرفق")]
        public string? FilePath { get; set; }

        public ICollection<Student>? Students { get; set; } // Many-to-Many
        public ICollection<Teacher>? Teachers { get; set; } // Many-to-Many
    }

}
