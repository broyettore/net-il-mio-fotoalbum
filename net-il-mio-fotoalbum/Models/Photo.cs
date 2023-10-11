using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using net_il_mio_fotoalbum.ValidationAttributes;

namespace net_il_mio_fotoalbum.Models
{
    [Table("Photos")]
    public class Photo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "You must insert a Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "You must insert a Description")]
        public string Description { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        [ValidateImgFile]
        public byte[]? ImageFile { get; set; }

        // N:N relation with categories
        public List<Category>? Categories { get; set; }

        public Photo() { }
    }
}
