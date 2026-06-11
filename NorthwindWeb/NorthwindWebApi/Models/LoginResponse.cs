namespace NorthwindWebApi.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; }
        public string NombreUsuario { get; set; }
        public string Rol { get; set; }
        public string? CustomerID { get; set; }
        public int? SupplierID { get; set; }
    }
}