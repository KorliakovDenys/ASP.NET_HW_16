using System.ComponentModel.DataAnnotations;

namespace Library.Models;

public class User {
    [Key] public int Id { get; set; }

    [Required] public string Login { get; set; }

    [Required] public string Password { get; set; }

    public Role Role { get; set; }
}