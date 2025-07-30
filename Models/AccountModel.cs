using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementSystem.Models;

public class AccountModel
{
    [Key]
    public string Id { get; set; }

    [Required]
    [DisplayName("Full Name")]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [DisplayName("Email Address")]
    public string Email { get; set; }

    [Required]
    [DisplayName("Password")]
    public string Password { get; set; }

    [DisplayName("Profile Icon")]
    public string ProfileIcon { get; set; }

    [NotMapped] 
    public IFormFile? UploadedImage { get; set; } // For upload
}
