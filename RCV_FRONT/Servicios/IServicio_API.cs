using RCV_FRONT.Models;

namespace RCV_FRONT.Servicios
{
    public interface IServicio_API
    { 
        Task<List<Vehiculo>> ListaV();
        Task<Vehiculo> ObtenerV(int idVehiculo);
        Task<bool> GuardarV(Vehiculo objeto);
        Task<bool> EditarV(Vehiculo objeto);
        Task<bool> EliminarV(int idVehiculo);

        
    }
}

