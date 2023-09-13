using System.ComponentModel.DataAnnotations;

namespace Library.Models;

public class Country {
    [Key] public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<City>? Cities { get; set; }
}