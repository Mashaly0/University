using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using University.Models;
namespace University.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // الجداول (DbSets)
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<EmailSettings> EmailSettings { get; set; }
        public DbSet<ContactViewModel> ContactViewModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student ↔ Course (Many-to-Many)
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Courses)
                .WithMany(c => c.Students)
                .UsingEntity(j => j.ToTable("StudentCourses"));

            // Student ↔ Room (Many-to-Many)
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Rooms)
                .WithMany(r => r.Students)
                .UsingEntity(j => j.ToTable("StudentRooms"));

            // Teacher ↔ Course (Many-to-Many)
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Courses)
                .WithMany(c => c.Teachers)
                .UsingEntity(j => j.ToTable("TeacherCourses"));

            // Teacher ↔ Room (One-to-Many)
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Rooms)
                .WithOne(r => r.Teacher)      // Room has a single Teacher
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete


            // SEED DATA 
            // ==========================

            // Teachers
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, Name = "Dr. Ahmed Ali", TeachertNum = 1001 },
                new Teacher { Id = 2, Name = "Dr. Mona Hassan", TeachertNum = 1002 },
                new Teacher { Id = 3, Name = "Dr. Omar Farid", TeachertNum = 1003 }
            );

            // Courses
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "Mathematics", IsAvailable = true },
                new Course { Id = 2, Name = "Physics", IsAvailable = true },
                new Course { Id = 3, Name = "Computer Science", IsAvailable = true },
                new Course { Id = 4, Name = "English Literature", IsAvailable = false }
            );

            // Students
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "Ali Mahmoud", StudentNum = 202101 },
                new Student { Id = 2, Name = "Sara Ibrahim", StudentNum = 202102 },
                new Student { Id = 3, Name = "Youssef Adel", StudentNum = 202103 },
                new Student { Id = 4, Name = "Nour El-Din", StudentNum = 202104 }
            );

            // Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Room A", Size = "Large", IsAvailable = true, TeacherId = 1 },
                new Room { Id = 2, Name = "Room B", Size = "Medium", IsAvailable = true, TeacherId = 2 },
                new Room { Id = 3, Name = "Room C", Size = "Small", IsAvailable = false, TeacherId = 1 },
                new Room { Id = 4, Name = "Room D", Size = "Large", IsAvailable = true, TeacherId = 3 }
            );
        }

    }


}

