using System.Security.Principal;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace MVC.Data;

public class DataContext : DbContext {
    public DbSet<City>? Cities { get; set; }
    public DbSet<Country>? Countries { get; set; }
    public DbSet<Course>? Courses { get; set; }
    public DbSet<Grade>? Grades { get; set; }
    public DbSet<Group>? Groups { get; set; }
    public DbSet<Manager>? Managers { get; set; }
    public DbSet<Student>? Students { get; set; }
    public DbSet<User>? Users { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<City>()
            .HasMany(c => c.Students)
            .WithOne(s => s.City)
            .HasForeignKey(s => s.CityId);

        modelBuilder.Entity<City>()
            .HasMany(c => c.Managers)
            .WithOne(m => m.City)
            .HasForeignKey(m => m.CityId);

        modelBuilder.Entity<Country>()
            .HasMany(c => c.Cities)
            .WithOne(c => c.Country)
            .HasForeignKey(c => c.CountryId);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Grades)
            .WithOne(g => g.Course)
            .HasForeignKey(g => g.CourseId);

        modelBuilder.Entity<Group>()
            .HasMany(g => g.Students)
            .WithOne(s => s.Group)
            .HasForeignKey(s => s.GroupId);

        modelBuilder.Entity<Manager>()
            .HasOne(m => m.City)
            .WithMany(c => c.Managers)
            .HasForeignKey(m => m.CityId);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.City)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CityId);
    }
    
    public bool IsUserAccessToStudentGranted(IPrincipal user, int studentId) {
        if (user.IsInRole("RegionalManager")) {
            var manager = Managers!.Include(m => m.User)
                .First(m => m.User!.Login == user.Identity!.Name);
            var student = Students!
                .First(s => s.Id == studentId);

            if (manager.CityId == student.CityId) {
                return true;
            }
        }
        else if (user.IsInRole("CentralManager")) {
            return true;
        }

        return false;
    }
}