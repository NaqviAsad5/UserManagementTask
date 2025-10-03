using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models.DTOs
{
   
    // For read
    public class UserReadDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

    // For write
    public class UserWriteDto
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,100}$",
        ErrorMessage = "Password must be 8–100 chars long, contain uppercase, lowercase, number, and special char.")]

        public string? Password { get; set; }
    }
}
