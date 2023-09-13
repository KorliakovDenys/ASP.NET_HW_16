using System.ComponentModel.DataAnnotations;

namespace Library.Models;

public class Group {
    [Key] public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<Student>? Students { get; set; }
}