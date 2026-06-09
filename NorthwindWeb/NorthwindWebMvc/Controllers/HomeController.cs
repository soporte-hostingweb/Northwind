using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NorthwindWebMvc.Models;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace NorthwindWebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("usuario") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Home model = new Home();
            List<Product> productosBajoStock = new List<Product>();

            using (var client = new HttpClient())
            {
                string apiBase = _configuration["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(apiBase);

                HttpResponseMessage response = await client.GetAsync("api/Home/getTotales");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<Home>(apiResponse);
                }

                // Obtener productos con bajo stock (menor o igual a 10 unidades)
                HttpResponseMessage prodResponse = await client.GetAsync("api/Product/getProductos");
                if (prodResponse.IsSuccessStatusCode)
                {
                    string prodApiResponse = await prodResponse.Content.ReadAsStringAsync();
                    var todosProductos = JsonConvert.DeserializeObject<List<Product>>(prodApiResponse);
                    if (todosProductos != null)
                    {
                        productosBajoStock = todosProductos
                            .Where(p => p.UnitsInStock.HasValue && p.UnitsInStock.Value <= 10)
                            .OrderBy(p => p.UnitsInStock.Value)
                            .Take(5)
                            .ToList();
                    }
                }
            }

            ViewBag.ProductosBajoStock = productosBajoStock;
            return View(model);
        }

        public IActionResult Privacy()
        {
            if (HttpContext.Session.GetString("usuario") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}