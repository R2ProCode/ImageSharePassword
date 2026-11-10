using ImageSharePassword.Data;
using ImageSharePassword.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ImageSharePassword.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString =
            @"Data Source=.\sqlexpress;Initial Catalog=ImageSharePassword;Integrated Security=true;Trust Server Certificate=true";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            var repo = new ImageSharePasswordRespository(_connectionString);

            string fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);
            //Console.WriteLine($"Full path: {fullPath}");
            using FileStream fs = new FileStream(fullPath, FileMode.Create);
            image.CopyTo(fs);

            var uploadedImage = new Image
            {
                ImagePath = fileName,
                Password = password
            };

            repo.AddImage(uploadedImage);
            return View(new UploadViewModel
            {
                Image = uploadedImage
            });
        }

        public IActionResult ViewImage(int id)
        {
            var repo = new ImageSharePasswordRespository(_connectionString);
            
            var viewModel = new ViewImageViewModel
            {
                Image = repo.GetImageById(id)
            };

            if (viewModel.Image == null)
            {
                return RedirectToAction("Index");
            }

            else if (TempData["message"] != null)
            {
                ViewBag.Message = (string)TempData["message"];
                
            }

            else if (HttpContext.Session.Get<List<int>>("images-ids-list") != null && HttpContext.Session.Get<List<int>>("images-ids-list").Contains(id))
            {
                repo.IncrementViews(id);
                viewModel.Image = repo.GetImageById(id);
                viewModel.Show = true;
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var repo = new ImageSharePasswordRespository(_connectionString);

            if (password != repo.GetPasswordForImage(id))
            {
                TempData["message"] = "Invalid password";
                return Redirect($"/Home/ViewImage?id={id}");
            }
            
            
            List<int> imagesAccessedIds = HttpContext.Session.Get<List<int>>("images-ids-list") ?? new List<int>();
            imagesAccessedIds.Add(id);
            HttpContext.Session.Set("images-ids-list", imagesAccessedIds);

            return Redirect($"/Home/ViewImage?id={id}");
        }
    }
}
