using NorthwindWebApi.Models;

namespace NorthwindWebApi.Repositorio.Interfaces
{
    public interface IUsuarioDAO
    {
        LoginResponse ValidarLogin(LoginRequest request);
    }
}