using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public int StudentNum { get; set; }
        public ICollection<Course>? Courses { get; set; }  // Many-to-Many with Course
        public ICollection<Room>? Rooms { get; set; }      // Many-to-Many with Room
    }
}
