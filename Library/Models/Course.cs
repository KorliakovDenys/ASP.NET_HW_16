using System.ComponentModel.DataAnnotations;

namespace Library.Models;

public class Course {
    [Key] public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<Grade>? Grades { get; set; }
}