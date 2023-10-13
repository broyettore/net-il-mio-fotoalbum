using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        private readonly PhotoContext _myDb;

        public MessagesController(PhotoContext myDb)
        {
            _myDb = myDb;
        }


        [HttpPost]
        [Route("Create/{photoId}")]
        public IActionResult Create(int photoId, [FromBody] CreateMessageRequest request)
        {
            try
            {
                // Find the photo based on the provided photoId
                var photo = _myDb.Photos.FirstOrDefault(p => p.Id == photoId);

                if (photo == null)
                {
                    return NotFound("Photo not found");
                }

                // Create a new message
                var message = new Message
                {
                    Email = request.Email,
                    MsgContent = request.MsgContent,
                    Photo = photo
                };

                _myDb.Messages.Add(message);
                _myDb.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        // Request model for creating a message
        public class CreateMessageRequest
        {
            public string Email { get; set; }
            public string MsgContent { get; set; }
        }



    }
}
