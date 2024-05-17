namespace RCV_FRONT.Models
{
    public class ResultadoCredencial
    {
       public string token { get; set; }
        public Usuario Usuario { get; internal set; }
    }
}
