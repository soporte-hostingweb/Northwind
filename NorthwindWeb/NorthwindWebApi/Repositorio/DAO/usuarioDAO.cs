using Microsoft.Data.SqlClient;
using NorthwindWebApi.Models;
using NorthwindWebApi.Repositorio.Interfaces;
using System.Data;

namespace NorthwindWebApi.Repositorio.DAO
{
    public class UsuarioDAO : IUsuarioDAO
    {
        private readonly IConfiguration _configuration;

        public UsuarioDAO(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LoginResponse ValidarLogin(LoginRequest request)
        {
            LoginResponse response = new LoginResponse
            {
                Success = false,
                Mensaje = "Usuario o contraseña incorrectos"
            };

            string cadena = _configuration.GetConnectionString("sql");

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                string sql = @"SELECT NombreUsuario, Rol, CustomerID, SupplierID
                               FROM Usuarios
                               WHERE NombreUsuario = @NombreUsuario
                               AND Clave = @Clave
                               AND Estado = 1";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", request.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Clave", request.Clave);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            response.Success = true;
                            response.Mensaje = "Login correcto";
                            response.NombreUsuario = dr["NombreUsuario"].ToString();
                            response.Rol = dr["Rol"].ToString();
                            response.CustomerID = dr["CustomerID"] != DBNull.Value ? dr["CustomerID"].ToString() : null;
                            response.SupplierID = dr["SupplierID"] != DBNull.Value ? Convert.ToInt32(dr["SupplierID"]) : null;
                        }
                    }
                }
            }

            return response;
        }
    }
}