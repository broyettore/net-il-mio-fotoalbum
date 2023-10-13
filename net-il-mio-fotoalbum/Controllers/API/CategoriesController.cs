using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly PhotoContext _myDb;

        public CategoriesController(PhotoContext myDb)
        {
            _myDb = myDb;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            List<Category> categories = _myDb.Categories.ToList();

            return Ok(categories);
        }
    }
}
