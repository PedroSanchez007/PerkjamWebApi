using System.ComponentModel.DataAnnotations;

namespace Perkjam.Model
{
    public class UserForUpdate
    {
        [Required]
        public string Name { get; set; }
    
        [Required]
        public string Email { get; set; }
    }
}