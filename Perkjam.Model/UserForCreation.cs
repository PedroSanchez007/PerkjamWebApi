using System.ComponentModel.DataAnnotations;

namespace Perkjam.Model
{
    public class UserForCreation
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Email { get; set; }
    }
}