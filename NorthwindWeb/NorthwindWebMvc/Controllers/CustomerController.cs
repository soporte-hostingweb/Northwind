using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NorthwindWebMvc.Models;
using System.Text;

namespace NorthwindWebMvc.Controllers
{
    public class CustomerController : Controller
    {
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("rol") != "Admin")
            {
                context.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(context);
        }

        private async Task<List<Cargo>> CargarCargos()
        {
            List<Cargo> listaCargos = new List<Cargo>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Cargo/");
                HttpResponseMessage response = await client.GetAsync("getCargos");
                string apiResponse = await response.Content.ReadAsStringAsync();
                listaCargos = JsonConvert.DeserializeObject<List<Cargo>>(apiResponse).ToList();
            }
            return listaCargos;
        }
        private async Task<List<Pais>> CargarPaises()
        {
            List<Pais> listaPaises = new List<Pais>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Pais/");
                HttpResponseMessage response = await client.GetAsync("getPaises");
                string apiResponse = await response.Content.ReadAsStringAsync();
                listaPaises = JsonConvert.DeserializeObject<List<Pais>>(apiResponse).ToList();
            }
            return listaPaises;
        }
        public async Task<IActionResult> Index(string? nombre, int page = 1, int pageSize = 10)
        {
            List<Customer> temporal = new List<Customer>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Customer/");
                string url = string.IsNullOrWhiteSpace(nombre) ? "getClientes" : $"getClientes?nombre={nombre}";
                HttpResponseMessage response = await client.GetAsync(url);
                string apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<Customer>>(apiResponse).ToList();
            }
            ViewBag.Nombre = nombre;
            var paged = new PagedList<Customer>
            {
                Items = temporal.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = temporal.Count
            };

            return View(paged);
        }

        public async Task<IActionResult> Create()
        {
            List<Cargo> listaCargos = await CargarCargos();
            List<Pais> listaPaises = await CargarPaises();
            ViewBag.Cargos = new SelectList(listaCargos, "CargoID", "NombreCargo");
            ViewBag.Paises = new SelectList(listaPaises, "PaisID", "NombrePais");
            return View(await Task.Run(() => new Customer()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer reg)
        {
            if (!ModelState.IsValid)
            {
                List<Cargo> listaCargos = await CargarCargos();
                List<Pais> listaPaises = await CargarPaises();
                ViewBag.Cargos = new SelectList(listaCargos, "CargoID", "NombreCargo");
                ViewBag.Paises = new SelectList(listaPaises, "PaisID", "NombrePais");
                return View(await Task.Run(() => reg));
            }

            string mensaje = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Customer/");
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(reg),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await client.PostAsync("insertCliente", content);
                mensaje = await response.Content.ReadAsStringAsync();
            }

            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            Customer reg = new Customer();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Customer/");
                HttpResponseMessage response = await client.GetAsync("getCliente/" + id);
                string apiResponse = await response.Content.ReadAsStringAsync();
                reg = JsonConvert.DeserializeObject<Customer>(apiResponse);
                List<Cargo> listaCargos = await CargarCargos();
                List<Pais> listaPaises = await CargarPaises();
                ViewBag.Cargos = new SelectList(listaCargos, "CargoID", "NombreCargo");
                ViewBag.Paises = new SelectList(listaPaises, "PaisID", "NombrePais");
            }

            return View(await Task.Run(() => reg));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer reg)
        {
            if (!ModelState.IsValid)
            {
                List<Cargo> listaCargos = await CargarCargos();
                List<Pais> listaPaises = await CargarPaises();
                ViewBag.Cargos = new SelectList(listaCargos, "CargoID", "NombreCargo");
                ViewBag.Paises = new SelectList(listaPaises, "PaisID", "NombrePais");
                return View(await Task.Run(() => reg));
            }

            string mensaje = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Customer/");
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(reg),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await client.PutAsync("updateCliente", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }

            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            Customer reg = new Customer();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Customer/");
                HttpResponseMessage response = await client.GetAsync("getCliente/" + id);
                string apiResponse = await response.Content.ReadAsStringAsync();
                reg = JsonConvert.DeserializeObject<Customer>(apiResponse);
            }

            return View(await Task.Run(() => reg));
        }

    }
}
