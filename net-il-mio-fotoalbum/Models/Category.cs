using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage= "You must insert a name")]
        [StringLength(50, ErrorMessage = "Name can not go over 50 chars")]
        public string Name { get; set; }

        // N:N relation with photos
        public List<Photo> Photos { get; set; }

        public Category() { }
    }
}
