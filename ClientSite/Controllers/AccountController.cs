﻿using ClientSite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ClientSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var loginRequest = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                // Make a POST request to the API for login
                var response = await client.PostAsync("https://localhost:7065/api/Auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<JwtTokenResponse>(responseContent);

                    // Store the JWT token in the session
                    _httpContextAccessor.HttpContext.Session.SetString("JWToken", tokenResponse.Token);

                    // Redirect to the Dashboard page upon successful login
                    return RedirectToAction("Dashboard", "Courses");
                }
                else
                {
                    // Add error message if login fails
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // Return to the login view if the login fails
            return View(model);
        }
    }
}