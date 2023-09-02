using CultureWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CultureWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin , SuperUser")]
   
    public class ReviewsController : Controller
    {
        private ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ReviewsController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
   
        public IActionResult PendingReviews()
        {
            var pendingReviews = _db.Reviews               
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => !r.IsApproved)
                .ToList();

            return View(pendingReviews);
        }

        [HttpPost]
        public IActionResult ApproveReview(int reviewId)
        {
            var review = _db.Reviews.Find(reviewId);
            if (review != null)
            {
                review.IsApproved = true;
                _db.Reviews.Update(review);
                _db.SaveChanges();
            }
            TempData["StatusMessage"] = "Review approved successfully!";
            return RedirectToAction(nameof(PendingReviews));
        }

        [HttpPost]
        public IActionResult RejectReview(int reviewId)
        {
            var review = _db.Reviews.Find(reviewId);
            if (review != null)
            {
                _db.Reviews.Remove(review);
                _db.SaveChanges();
            }
            TempData["StatusMessage"] = "Review was rejected !";
            return RedirectToAction(nameof(PendingReviews));
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pendingReview = _db.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id && !r.IsApproved);

            if (pendingReview == null)
            {
                return NotFound();
            }

            return View(pendingReview);
        }


    }
}
