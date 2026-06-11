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
                if (!string.IsNullOrEmpty(respuesta.CustomerID))
                {
                    HttpContext.Session.SetString("customerID", respuesta.CustomerID);
                }
                if (respuesta.SupplierID.HasValue)
                {
                    HttpContext.Session.SetInt32("supplierID", respuesta.SupplierID.Value);
                }

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

        [HttpPost]
        public IActionResult ImpersonateUser(string customerId, string supplierId, string userName)
        {
            var userRole = HttpContext.Session.GetString("rol");
            if (userRole == "Admin")
            {
                HttpContext.Session.SetString("AdminOriginalUsuario", HttpContext.Session.GetString("usuario"));
                HttpContext.Session.SetString("AdminOriginalRol", "Admin");

                HttpContext.Session.SetString("usuario", userName);
                HttpContext.Session.SetString("rol", "Cliente");

                if (!string.IsNullOrEmpty(customerId))
                {
                    HttpContext.Session.SetString("customerID", customerId);
                }
                else
                {
                    HttpContext.Session.Remove("customerID");
                }

                if (!string.IsNullOrEmpty(supplierId) && int.TryParse(supplierId, out int supId))
                {
                    HttpContext.Session.SetInt32("supplierID", supId);
                }
                else
                {
                    HttpContext.Session.Remove("supplierID");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult StopImpersonating()
        {
            var originalAdmin = HttpContext.Session.GetString("AdminOriginalUsuario");
            if (!string.IsNullOrEmpty(originalAdmin))
            {
                HttpContext.Session.SetString("usuario", originalAdmin);
                HttpContext.Session.SetString("rol", HttpContext.Session.GetString("AdminOriginalRol") ?? "Admin");

                HttpContext.Session.Remove("AdminOriginalUsuario");
                HttpContext.Session.Remove("AdminOriginalRol");
                HttpContext.Session.Remove("customerID");
                HttpContext.Session.Remove("supplierID");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}