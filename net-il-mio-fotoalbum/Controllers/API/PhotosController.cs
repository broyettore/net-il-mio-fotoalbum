using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly PhotoContext _myDb;

        public PhotosController(PhotoContext myDb)
        {
            _myDb = myDb;
        }

        [HttpGet]
        public IActionResult GetPhotos()
        {
            List<Photo> photos = _myDb.Photos.Include(p => p.Categories).ToList();

            return Ok(photos);
        }

        [HttpGet]
        public IActionResult GetPhotosByTitle(string? title)
        {
            if (title == null)
            {
                return BadRequest(new { Message = "No parameter inserted" });
            }

            List<Photo> foundPhotos = _myDb.Photos.Where(p => p.Title.ToLower().Contains(title.ToLower())).Include(p => p.Categories).ToList();

            return Ok(foundPhotos);
        }

        [HttpGet("{id}")]
        public IActionResult GetPhotoById(int id)
        {
            Photo? foundPhoto = _myDb.Photos.Where(p => p.Id == id).Include(p => p.Categories).FirstOrDefault();

            if (foundPhoto != null)
            {
                return Ok(foundPhoto);
            }
            else
            {
                return NotFound($"the photo with id {id} was not found");
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Photo newPhoto)
        {
            try
            {
                _myDb.Photos.Add(newPhoto);
                _myDb.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Modify(int id, [FromBody] Photo updatedPhoto)
        {
            Photo? photoToEdit = _myDb.Photos.Where(p => p.Id == id).FirstOrDefault();

            if (photoToEdit != null)
            {
                photoToEdit.Title = updatedPhoto.Title;
                photoToEdit.Description = updatedPhoto.Description;
                photoToEdit.ImageUrl = updatedPhoto.ImageUrl;
                photoToEdit.IsVisible = updatedPhoto.IsVisible;


                _myDb.SaveChanges();

                return Ok();
            }
            else
            {
                return NotFound($"the photo to edit with id {id} was not found");
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Photo? photoToDelete = _myDb.Photos.Where(p => p.Id == id).FirstOrDefault();

            if (photoToDelete == null)
            {
                return NotFound($"the photo to delete with id {id} was not found");
            }
            else
            {
                _myDb.Photos.Remove(photoToDelete);
                _myDb.SaveChanges();
                return Ok();
            }
        }
    }
}
