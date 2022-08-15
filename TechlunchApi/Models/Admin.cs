using System.ComponentModel.DataAnnotations;

namespace TechlunchApi.Models
{
    public class Admin
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
