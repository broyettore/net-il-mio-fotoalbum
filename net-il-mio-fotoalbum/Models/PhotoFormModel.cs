using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoFormModel
    {
        public Photo Photo { get; set; }

        public IFormFile? ImgFormFile { get; set; }



        // multiple select for categories
        public List<SelectListItem>? SelectCategories { get; set; }
        public List<string>? SelectedCategoriesIds { get; set; }

    }
}
