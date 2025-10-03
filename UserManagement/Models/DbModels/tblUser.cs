using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models.DbModels
{
    public class tblUser
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
