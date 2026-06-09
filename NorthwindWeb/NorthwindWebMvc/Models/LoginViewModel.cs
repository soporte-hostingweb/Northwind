using System.ComponentModel.DataAnnotations;

namespace NorthwindWebMvc.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese el usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "Ingrese la clave")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}