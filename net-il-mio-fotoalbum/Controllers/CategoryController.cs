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
            List<Category> categories = _myDb.Categories.ToList();
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
                // Log detailed validation errors
                var validationErrors = ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(kvp => kvp.Key,
                                  kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList());

                _logger.LogWarning("Invalid model state for creating a category. Validation errors: {@ValidationErrors}", validationErrors);

                return View("Create", newCategory);
            }

            try
            {
                _myDb.Categories.Add(newCategory);
                _myDb.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a category: {@NewCategory}", newCategory);
                throw;  // Re-throw the exception to display a generic error page
            }
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

