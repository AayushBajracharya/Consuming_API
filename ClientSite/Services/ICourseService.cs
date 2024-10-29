using ClientSite.Models;

namespace ClientSite.Services
{
    public interface ICourseService
    {

        Task<IEnumerable<Course>> GetCoursesAsync(string token);
        Task<Course> GetCourseByIdAsync(int id, string token);
        Task<string> AddCourseAsync(Course course, IFormFile imageFile, string token);
        Task<string> UpdateCourseAsync(int id, Course course, IFormFile imageFile, string token);
        Task<string> DeleteCourseAsync(int id, string token);
    }
}
