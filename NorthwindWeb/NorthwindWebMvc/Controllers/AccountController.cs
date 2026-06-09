using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NorthwindWebMvc.Models;
using System.Text;

namespace NorthwindWebMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            LoginResponse respuesta = new LoginResponse();

            using (var client = new HttpClient())
            {
                string apiBase = _configuration["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(apiBase);

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("api/Usuario/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    respuesta = JsonConvert.DeserializeObject<LoginResponse>(apiResponse);
                }
            }

            if (respuesta != null && respuesta.Success)
            {
                HttpContext.Session.SetString("usuario", respuesta.NombreUsuario);
                HttpContext.Session.SetString("rol", respuesta.Rol);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = respuesta?.Mensaje ?? "No se pudo iniciar sesión";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}