using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models;

public class Manager {
    [Key] public int Id { get; set; }
    
    public string? FullName { get; set; }
    
    public ManagerRole Role { get; set; }

    public int UserId { get; set; }
    
    [ForeignKey("UserId")] public User? User { get; set; }

    public int CityId { get; set; }

    [ForeignKey("CityId")] public City? City { get; set; }
}