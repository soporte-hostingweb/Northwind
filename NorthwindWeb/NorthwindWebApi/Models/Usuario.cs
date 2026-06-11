namespace NorthwindWebApi.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }
        public string? CustomerID { get; set; }
        public int? SupplierID { get; set; }
    }
}