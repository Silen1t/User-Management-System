using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models;

public class SignInModel
{
    [Required]
    [EmailAddress]
    [DisplayName("Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DisplayName("Password")]
    public string Password { get; set; } = string.Empty;
}
