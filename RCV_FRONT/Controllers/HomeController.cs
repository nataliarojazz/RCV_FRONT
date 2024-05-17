using Microsoft.AspNetCore.Mvc;
using RCV_FRONT.Models;
using System.Diagnostics;
using RCV_FRONT.Servicios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using IronBarCode;
using System.Drawing;
using System.Net.Http;

namespace RCV_FRONT.Controllers
{
    public class HomeController : Controller
    {

        private readonly IServicio_API _servicio_API;
        private readonly IServicioU_API _servicioU_API;
        private readonly IWebHostEnvironment _iwebHostEnvironment;
        private readonly HttpClient _httpClient;


        public HomeController(IServicio_API servicio_API, IServicioU_API servicioU_API, IWebHostEnvironment webHostEnvironment, HttpClient httpClient)
        {
            _servicio_API = servicio_API;
            _servicioU_API = servicioU_API;
            _iwebHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new System.Uri("http://localhost:5071/"); // Reemplaza "https://example.com" con la base URL de tu API

        }



        public async Task<IActionResult> Index()
        {
            List<Vehiculo> ListaV = await _servicio_API.ListaV();
            return View(ListaV);
        }

        public async Task<IActionResult> UsuarioL()
        {
            List<Usuario> ListaU = await _servicioU_API.ListaU();
            return View(ListaU);
        }

        public async Task<IActionResult> Usuario(int idUsuario)
        {
            Usuario modelo_usuario = new Usuario();
            ViewBag.Accion = "Nueva Usuario";

            if (idUsuario == 0)
            {
                modelo_usuario = await _servicioU_API.ObtenerU(idUsuario);
                ViewBag.Accion = "Editar Usuario";
            }
            return View(modelo_usuario);
        }

        [HttpGet]
        public IActionResult GeneratePdf()
        {
            return View();
        }

        public async Task<IActionResult> GeneratePdf(string reporte, int idUsuario)
        {
            var requestUrl = "api/Pdf/GenerarPDF?reporte=" + reporte + "&idUsuario=" + idUsuario;


            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentDisposition = response.Content.Headers.ContentDisposition;
                    var fileName = contentDisposition?.FileName ?? "Reporte.pdf";

                    return File(pdfBytes, "application/pdf", fileName);
                }
                else
                {
                    ViewBag.ErrorMessage = "Error al generar el PDF: " + await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException e)
            {
                ViewBag.ErrorMessage = "Error al enviar la solicitud: " + e.Message;
            }

            return View("Index");
        }
    



    [HttpPost]
        public async Task<IActionResult> GuardarUsuario(Usuario ob_Usuario)
        {
            bool respuesta;
            if (ob_Usuario.IdUsuario == 0)
            {
                respuesta = await _servicioU_API.GuardarU(ob_Usuario);
            }
            else
            {
                respuesta = await _servicioU_API.EditarU(ob_Usuario);
            }
            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
                return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> EliminarU(int idUsuario)
        {
            var respuesta = await _servicioU_API.EliminarU(idUsuario);

            if (respuesta)
                return RedirectToAction("Index");
            else
                return NoContent();
        }


        public async Task<IActionResult> Vehiculo(int idVehiculo)
        {
            Vehiculo modelo_vehiculo = new Vehiculo();
            ViewBag.Accion = "Nueva Vehiculo";

            if (idVehiculo == 0)
            {
                modelo_vehiculo = await _servicio_API.ObtenerV(idVehiculo);
                ViewBag.Accion = "Editar Vehiculo";
            }
            return View(modelo_vehiculo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarV(Vehiculo ob_Vehiculo)
        {
            bool respuesta;
            if (ob_Vehiculo.IdVehiculo == 0)
            {
                respuesta = await _servicio_API.GuardarV(ob_Vehiculo);
            }
            else
            {
                respuesta = await _servicio_API.EditarV(ob_Vehiculo);
            }
            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
                return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> EliminarV(int idVehiculo)
        {
            var respuesta = await _servicio_API.EliminarV(idVehiculo);

            if (respuesta)
                return RedirectToAction("Index");
            else
                return NoContent();
        }



        public IActionResult CreateQRCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateQRCode(GenerateQRCodeModel GenerateQRCode)
        {
            try
            {
                GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(GenerateQRCode.QRCodeText, 200);
                barcode.AddBarcodeValueTextBelowBarcode();
                barcode.SetMargins(10);
                barcode.ChangeBarCodeColor(Color.Black);
                string path = Path.Combine(_iwebHostEnvironment.WebRootPath, "GeneratedQRCode");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = Path.Combine(_iwebHostEnvironment.WebRootPath, "GeneratedQRCode/qrcode.png");
                barcode.SaveAsPng(filePath);
                string filename = Path.GetFileName(filePath);
                string imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/GeneratedQRCode/" + filename;
                ViewBag.QrCodeUri = imageUrl;
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
