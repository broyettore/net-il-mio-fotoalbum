using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;
using System.Linq;
using System.Threading.Tasks;

namespace net_il_mio_fotoalbum.Controllers
{
    public class CategoryController : Controller
    {
        private readonly PhotoContext _myDb;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(PhotoContext db, ILogger<CategoryController> logger)
        {
            _myDb = db;
            _logger = logger;
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult Index()
        {
            List<Category> categories = _myDb.Categories.Include(c => c.Photos).ToList();
            return View("Index", categories);
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Create(Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", newCategory);
            }

            _myDb.Categories.Add(newCategory);
            _myDb.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            Category? categoryToEdit = _myDb.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (categoryToEdit == null)
            {
                return NotFound("The category to edit was not found");
            }
            else
            {
                return View("Update", categoryToEdit);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Update(int id, Category modifiedCategory)
        {
            if(!ModelState.IsValid)
            {
                return View("Update", modifiedCategory);
            }

            Category? categoryToUpdate = _myDb.Categories.Where(c => c.Id == id).FirstOrDefault();

            if(categoryToUpdate != null)
            {
                categoryToUpdate.Name = modifiedCategory.Name;

                _myDb.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return NotFound("The category to delete was not found");
            }
        }


        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Category? categoryToDelete = _myDb.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (categoryToDelete != null)
            {
                _myDb.Categories.Remove(categoryToDelete);
                _myDb.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound("The category to delete was not found");
            }
        }
    }
}

