using ClientSite.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ClientSite.Services
{
    public class CourseService : ICourseService
    {
        private readonly HttpClient _httpClient;

        public CourseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("https://localhost:7065/api/ControllerCourse");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Course>>();
        }

        public async Task<Course> GetCourseByIdAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"https://localhost:7065/api/ControllerCourse/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Course>();
        }

        public async Task<string> AddCourseAsync(Course course, IFormFile imageFile, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formData = new MultipartFormDataContent
    {
        { new StringContent(course.Name), "Name" },
        { new StringContent(course.Description), "Description" }
    };

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(stream);
                    var imageContent = new ByteArrayContent(stream.ToArray());
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                    formData.Add(imageContent, "productImage", imageFile.FileName); // Ensure this key matches the API
                }
            }

            var response = await _httpClient.PostAsync("https://localhost:7065/api/ControllerCourse", formData);

            if (response.IsSuccessStatusCode)
            {
                return "Course added successfully!";
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return $"Error adding course: {errorMessage}";
            }
        }

        // Update a product
        public async Task<string> UpdateCourseAsync(int id, Course course, IFormFile imageFile, string token)
        {
            // Use _httpClient instead of creating a new instance
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Prepare the multipart form data content
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(course.Name), "Name");
            content.Add(new StringContent(course.Description), "Description");

            // Handle image file if provided
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = new MemoryStream();
                await imageFile.CopyToAsync(stream);
                var imageContent = new ByteArrayContent(stream.ToArray());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(imageContent, "productImage", imageFile.FileName);
            }

            // Interpolate id correctly in the URL
            var response = await _httpClient.PutAsync($"https://localhost:7065/api/ControllerCourse/{id}", content);

            // Return null if successful, otherwise return the error reason
            return response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync();
        }


        public async Task<string> DeleteCourseAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"https://localhost:7065/api/ControllerCourse/{id}");
            return response.IsSuccessStatusCode
                ? "Course deleted successfully!"
                : "Error deleting course: " + await response.Content.ReadAsStringAsync();
        }
    }
}
