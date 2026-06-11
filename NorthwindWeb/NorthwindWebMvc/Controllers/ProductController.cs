using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NorthwindWebMvc.Models;
using System.Text;

namespace NorthwindWebMvc.Controllers
{
    public class ProductController : Controller
    {
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("rol") != "Admin")
            {
                context.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(context);
        }

        private async Task<List<Supplier>> CargarProveedores()
        {
            List<Supplier> listaProveedores = new List<Supplier>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Supplier/");
                HttpResponseMessage response = await client.GetAsync("getProveedores");
                string apiResponse = await response.Content.ReadAsStringAsync();
                listaProveedores = JsonConvert.DeserializeObject<List<Supplier>>(apiResponse).ToList();
            }
            return listaProveedores;
        }
        private async Task<List<Category>> CargarCategorias()
        {
            List<Category> listaCategorias = new List<Category>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Category/");
                HttpResponseMessage response = await client.GetAsync("getCategorias");
                string apiResponse = await response.Content.ReadAsStringAsync();
                listaCategorias = JsonConvert.DeserializeObject<List<Category>>(apiResponse).ToList();
            }
            return listaCategorias;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string producto = "")
        {
            List<Product> temporal = new List<Product>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Product/");
                HttpResponseMessage response = await client.GetAsync($"getProductos?producto={producto}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<Product>>(apiResponse).ToList();
            }

            var paged = new PagedList<Product>
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
            List<Supplier> listaProveedores = await CargarProveedores();
            List<Category> listaCategorias = await CargarCategorias();
            ViewBag.Proveedores = new SelectList(listaProveedores, "SupplierID", "CompanyName");
            ViewBag.Categorias = new SelectList(listaCategorias, "CategoryID", "CategoryName");
            return View(await Task.Run(() => new Product()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product reg)
        {
            if (!ModelState.IsValid)
            {
                List<Supplier> listaProveedores = await CargarProveedores();
                List<Category> listaCategorias = await CargarCategorias();
                ViewBag.Proveedores = new SelectList(listaProveedores, "SupplierID", "CompanyName");
                ViewBag.Categorias = new SelectList(listaCategorias, "CategoryID", "CategoryName");
                return View(await Task.Run(() => reg));
            }
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Product/");
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("insertProducto", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }
            Product reg = new Product();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Product/");
                HttpResponseMessage response = await client.GetAsync("getProducto/" + id);
                string apiResponse = await response.Content.ReadAsStringAsync();
                reg = JsonConvert.DeserializeObject<Product>(apiResponse);
                List<Supplier> listaProveedores = await CargarProveedores();
                List<Category> listaCategorias = await CargarCategorias();
                ViewBag.Proveedores = new SelectList(listaProveedores, "SupplierID", "CompanyName");
                ViewBag.Categorias = new SelectList(listaCategorias, "CategoryID", "CategoryName");
            }
            return View(await Task.Run(() => reg));
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product reg)
        {
            if (!ModelState.IsValid)
            {
                List<Supplier> listaProveedores = await CargarProveedores();
                List<Category> listaCategorias = await CargarCategorias();
                ViewBag.Proveedores = new SelectList(listaProveedores, "SupplierID", "CompanyName");
                ViewBag.Categorias = new SelectList(listaCategorias, "CategoryID", "CategoryName");
                return View(await Task.Run(() => reg));
            }
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Product/");
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("updateProducto", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
                List<Supplier> listaProveedores = await CargarProveedores();
                List<Category> listaCategorias = await CargarCategorias();
                ViewBag.Proveedores = new SelectList(listaProveedores, "SupplierID", "CompanyName");
                ViewBag.Categorias = new SelectList(listaCategorias, "CategoryID", "CategoryName");
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }
            Product reg = new Product();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Product/");
                HttpResponseMessage response = await client.GetAsync("getProducto/" + id);
                string apiResponse = await response.Content.ReadAsStringAsync();
                reg = JsonConvert.DeserializeObject<Product>(apiResponse);
            }
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7226/api/Product/");
                HttpResponseMessage response = await client.DeleteAsync("deleteProducto/" + id);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            TempData["mensaje"] = mensaje;
            return RedirectToAction("Index");
        }
    }
}
