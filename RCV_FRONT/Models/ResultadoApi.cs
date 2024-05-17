namespace RCV_FRONT.Models
{
    public class ResultadoApi
    {
        public string mensaje { get; set; }
        public List<Vehiculo> responseV { get; set;}
        public List<Usuario> response { get; set; }
        public Usuario objetoU { get; set; }
        public Vehiculo objetoV { get; set; }
    }
}
