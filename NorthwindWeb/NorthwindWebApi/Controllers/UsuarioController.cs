using Microsoft.AspNetCore.Mvc;
using NorthwindWebApi.Models;
using NorthwindWebApi.Repositorio.Interfaces;

namespace NorthwindWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioDAO _usuarioDAO;

        public UsuarioController(IUsuarioDAO usuarioDAO)
        {
            _usuarioDAO = usuarioDAO;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = _usuarioDAO.ValidarLogin(request);
            return Ok(response);
        }
    }
}