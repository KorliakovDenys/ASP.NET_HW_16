using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models;

public class Grade {
    [Key] public int Id { get; set; }

    public int Value { get; set; }

    public int StudentId { get; set; }

    [ForeignKey("StudentId")] public Student? Student { get; set; }

    public int CourseId { get; set; }

    [ForeignKey("CourseId")] public Course? Course { get; set; }
}