using System.ComponentModel.DataAnnotations;

namespace SecurityLib.Models
{
    public class Credential
    {
        [Key]
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public DateTime ModifiedDate { get; set; } 
    }
}