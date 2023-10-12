using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Models;
using net_il_mio_fotoalbum.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        private readonly PhotoContext _myDb;
        private readonly ILogger<PhotoController> _logger;

        public PhotoController(PhotoContext db, ILogger<PhotoController> logger)
        {
            _myDb = db;
            _logger = logger;
        }

        [Authorize(Roles = "ADMIN")]
        public IActionResult Index()
        {
           List<Photo> photos = _myDb.Photos.Include(p => p.Categories).ToList();

            return View("Index", photos);
        } 
        
        public IActionResult UserIndex()
        {
           List<Photo> photos = _myDb.Photos.Include(p => p.Categories).ToList();

            return View("UserIndex", photos);
        }

        public IActionResult Details(int id)
        {
            Photo? foundPhoto = _myDb.Photos.Where(p =>  p.Id == id)
                .Include(p => p.Categories)
                .FirstOrDefault();

            if(foundPhoto == null)
            {
                return NotFound($"The photo with id {id} was not found");
            } else
            {
                return View("Details", foundPhoto);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Create()
        {
            List<Category> categories = _myDb.Categories.ToList();
            List<SelectListItem> AllCategoriesSelectList = new();

            foreach (Category category in categories)
            {
                AllCategoriesSelectList.Add(
                    new SelectListItem
                    {
                        Text = category.Name,
                        Value = category.Id.ToString(),
                    });
            }


            PhotoFormModel model = new()
            {
                Photo = new Photo(),
                SelectCategories = AllCategoriesSelectList
            };

            return View("Create", model);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoFormModel data)
        {
            if(!ModelState.IsValid)
            {
                List<Category> categories = _myDb.Categories.ToList();
                List<SelectListItem> AllCategoriesSelectList = new();

                foreach (Category category in categories)
                {
                    AllCategoriesSelectList.Add(
                        new SelectListItem
                        {
                            Text = category.Name,
                            Value = category.Id.ToString(),
                        });
                }

                data.SelectCategories = AllCategoriesSelectList;

                return View("Create", data);
            }

            data.Photo.Categories = new List<Category>();

            if (data.SelectedCategoriesIds != null)
            {
                foreach (string selectedCategoryId in data.SelectedCategoriesIds)
                {
                    if (int.TryParse(selectedCategoryId, out int intSelectedCategoryId))
                    {
                        Category? categoryInDb = _myDb.Categories.FirstOrDefault(c => c.Id == intSelectedCategoryId);

                        if (categoryInDb != null)
                        {
                            data.Photo.Categories.Add(categoryInDb);
                        }
                    }
                }
            }

            this.SetImageFileFromFile(data);

            _myDb.Photos.Add(data.Photo);
            _myDb.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Update(int id)
        {

            Photo? photoToEdit = _myDb.Photos.Where(p => p.Id == id).Include(p => p.Categories).FirstOrDefault();

            if (photoToEdit == null)
            {
                return NotFound("Photo to edit was not found");
            }
            else
            {
                List<Category> categories = _myDb.Categories.ToList();
                List<SelectListItem> AllCategoriesSelectList = new();

                foreach (Category category in categories)
                {
                    AllCategoriesSelectList.Add(
                        new SelectListItem
                        {
                            Text = category.Name,
                            Value = category.Id.ToString(),
                            Selected = photoToEdit.Categories.Any(relatedCategory => relatedCategory.Id == category.Id)
                        });
                }

                PhotoFormModel model = new()
                {
                    Photo = photoToEdit,
                    SelectCategories = AllCategoriesSelectList,
                };

                return View("Update", model);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, PhotoFormModel data)
        {
            _logger.LogInformation("Attempting to update pizza with ID: {id}", id);

            if (!ModelState.IsValid)
            {
                List<Category> categories = _myDb.Categories.ToList();
                List<SelectListItem> AllCategoriesSelectList = new();

                foreach (Category category in categories)
                {
                    AllCategoriesSelectList.Add(
                        new SelectListItem
                        {
                            Text = category.Name,
                            Value = category.Id.ToString(),
                        });
                }

                data.SelectCategories = AllCategoriesSelectList;

                return View("Update", data);
            }


            Photo? photoToEdit = _myDb.Photos.Where(p => p.Id == id).Include(p => p.Categories).FirstOrDefault();

            if (photoToEdit != null)
            {
                photoToEdit.Categories.Clear();

                photoToEdit.Title = data.Photo.Title;
                photoToEdit.Description = data.Photo.Description;
                photoToEdit.ImageUrl = data.Photo.ImageUrl;
                photoToEdit.IsVisible = data.Photo.IsVisible;

                if (data.SelectedCategoriesIds != null)
                {
                    foreach (string selectedCategoryId in data.SelectedCategoriesIds)
                    {
                        int intSelectedCategoryId = int.Parse(selectedCategoryId);

                        Category? categoryInDb = _myDb.Categories.Where(c => c.Id == intSelectedCategoryId).FirstOrDefault();

                        if (categoryInDb != null)
                        {
                            photoToEdit.Categories.Add(categoryInDb);
                        }
                    }
                }

                if(data.ImgFormFile != null)
                {
                    MemoryStream stream = new();
                    data.ImgFormFile.CopyTo(stream);
                    photoToEdit.ImageFile = stream.ToArray();
                }

                _myDb.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogWarning("Photo not found for update. ID: {id}", id);
                return NotFound("The photo you want to edit was not found");
            }

        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Photo? photoToDelete = _myDb.Photos.Where(p => p.Id == id).FirstOrDefault();

            if (photoToDelete != null)
            {
                _myDb.Photos.Remove(photoToDelete);
                _myDb.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound("The pizza to delete was not found");
            }
        }

        private void SetImageFileFromFile(PhotoFormModel data)
        {
            if(data.ImgFormFile == null)
            {
                return;
            }

            MemoryStream stream = new();
            data.ImgFormFile.CopyTo(stream);
            data.Photo.ImageFile = stream.ToArray();
        }
    }
}
