using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.Models
{
    public class Room
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public ICollection<Student>? Students { get; set; } // Many-to-Many
        public int TeacherId { get; set; } // FK
        public Teacher? Teacher { get; set; }
    }
}
