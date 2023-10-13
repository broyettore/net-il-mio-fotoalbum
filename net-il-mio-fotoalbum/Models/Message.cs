using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message content is required")]
        public string MsgContent { get; set; }

        // Foreign key for the related photo
        public int PhotoId { get; set; }

        // Navigation property to represent the related photo
        public Photo Photo { get; set; }
    }
}
