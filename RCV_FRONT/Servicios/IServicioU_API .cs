using RCV_FRONT.Models;

namespace RCV_FRONT.Servicios
{
    public interface IServicioU_API
    { 
        Task<List<Usuario>> ListaU();
        Task<Usuario> ObtenerU(int idUsuario);
        Task<bool> GuardarU(Usuario objeto);
        Task<bool> EditarU(Usuario objeto);
        Task<bool> EliminarU(int idUsuario);

        Task<Usuario> GetUsuario(string Documento, string Clave);
    }
}

