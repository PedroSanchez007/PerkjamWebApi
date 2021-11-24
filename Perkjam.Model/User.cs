using System.ComponentModel.DataAnnotations;

namespace Perkjam.Model
{
    using System;
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
