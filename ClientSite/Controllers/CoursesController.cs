using ClientSite.Models;
using ClientSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ClientSite.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CoursesController(ICourseService courseService, IHttpContextAccessor httpContextAccessor)
        {
            _courseService = courseService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(int? courseId = null)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var courses = await _courseService.GetCoursesAsync(token);
            ViewBag.Courses = courses;

            if (courseId.HasValue)
            {
                var courseToEdit = courses.FirstOrDefault(p => p.Id == courseId.Value);
                ViewBag.CourseToEdit = courseToEdit; // Pass the product to edit to the view
                var courseToDelete = courses.FirstOrDefault(p => p.Id == courseId.Value);
                ViewBag.CourseToDelete = courseToDelete; // Pass the product to Delete to the view
            }

            return View(courses);
        }


        [HttpPost]
        public async Task<IActionResult> AddCourse(Course course, IFormFile productImage)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Call the AddCourseAsync method to handle the addition of the course
            var result = await _courseService.AddCourseAsync(course, productImage, token);

            // Check if the result indicates success or failure
            if (result != "Course added successfully!")
            {
                ModelState.AddModelError("", result);
                return View("Dashboard", await _courseService.GetCoursesAsync(token));
            }

            return RedirectToAction("Dashboard");
        }



        [HttpPost]
        public async Task<IActionResult> EditCourse(Course course, IFormFile productImage)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            /* if (ModelState.IsValid)
             {*/
            // Attempt to update the product through the API
            var result = await _courseService.UpdateCourseAsync(course.Id, course, productImage, token);

            if (string.IsNullOrEmpty(result)) // Check if update was successful (result should be null if no error)
            {
                return RedirectToAction("Dashboard");
            }

            // Set the error message if the update fails
            ModelState.AddModelError(string.Empty, result);
            /* }*/

            // Reload products and add error message to ViewBag
            ViewBag.Error = "Failed to update course.";
            ViewBag.Courses = await _courseService.GetCoursesAsync(token); // Pass the products for the dashboard view

            return View("Dashboard");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var result = await _courseService.DeleteCourseAsync(id, token);
            ModelState.AddModelError("", result);
            return RedirectToAction("Dashboard");
        }
    }
}


