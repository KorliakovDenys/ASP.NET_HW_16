using System.ComponentModel.DataAnnotations;

namespace MVC.Models; 

public class RegisterModel {
    [Required(ErrorMessage = "Email is not specified")]
    [DataType(DataType.EmailAddress)]
    public string Login { get; set; }

    [Required(ErrorMessage = "Password is not specified")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password is entered incorrectly")]
    public string ConfirmPassword { get; set; }
}
