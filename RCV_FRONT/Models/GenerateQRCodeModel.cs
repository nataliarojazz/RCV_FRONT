using System.ComponentModel.DataAnnotations;

namespace RCV_FRONT.Models
{
    public class GenerateQRCodeModel
    {
        [Display(Name = "Ingrese el texto")]
        public string QRCodeText { get; set; }
    }
}
