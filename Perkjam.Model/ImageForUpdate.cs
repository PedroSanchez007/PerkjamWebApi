using System.ComponentModel.DataAnnotations;

namespace Perkjam.Model
{
    public class ImageForUpdate
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }      
    }
}
