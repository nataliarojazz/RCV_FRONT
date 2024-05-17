using Microsoft.AspNetCore.Mvc;
using RCV_FRONT.Models;
using System.Diagnostics;
using RCV_FRONT.Servicios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace RCV_FRONT.Controllers
{
    public class InicioController : Controller
    {
     
        private static string _baseurl;


        public InicioController()
        {
           
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            _baseurl = builder.GetSection("ApiSettings:baseUrl").Value;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }
        public async Task<IActionResult> Registrarse(Usuario objetoU)
        {
            try
            {
                // Llamar al método para guardar el usuario en la API
                bool guardadoExitoso = await GuardarU(objetoU);

                if (guardadoExitoso)
                {
                    // Si el guardado es exitoso, redirigir al usuario a la página de inicio de sesión
                    return RedirectToAction("IniciarSesion", "Inicio");
                }
                else
                {
                    // Si ocurre algún problema durante el guardado, mostrar un mensaje de error en la vista
                    ViewData["Mensaje"] = "Error al guardar el usuario. Por favor, inténtalo de nuevo.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante el proceso de guardado
                ViewData["Mensaje"] = $"Error al guardar el usuario: {ex.Message}";
                return View();
            }
        }

        public async Task<bool> GuardarU(Usuario objetoU)
        {
            bool respuesta = false;

            // Aquí puedes incluir la lógica para autenticar si es necesario antes de guardar el usuario

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(_baseurl);
                // Agregar encabezado de autorización si es necesario
                // cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                // Convertir el objeto usuario a JSON
                var content = new StringContent(JsonConvert.SerializeObject(objetoU), Encoding.UTF8, "application/json");

                // Enviar la solicitud HTTP POST para guardar el usuario en la API
                var response = await cliente.PostAsync("api/Usuario/GuardarU", content);

                // Verificar si la respuesta es exitosa
                if (response.IsSuccessStatusCode)
                {
                    respuesta = true;
                }
            }

            return respuesta;
        }

        [HttpGet]
        public async Task<IActionResult> CerrarSesion()
        {
            // Cerrar la sesión del usuario
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirigir al usuario a la página de inicio de sesión
            return RedirectToAction("IniciarSesion", "Inicio");
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        public async Task<IActionResult> IniciarSesion(string documento, string clave)
        {
            try
            {
                string uri = $"{_baseurl}api/Autenticacion/Validar";

                // Crear los datos de la solicitud
                var datos = new Dictionary<string, string>
        {
            { "Documento", documento },
            { "Clave", clave }
        };

                // Serializar los datos a JSON
                var json = JsonConvert.SerializeObject(datos);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Enviar la solicitud HTTP POST con los datos en el cuerpo
                using (var cliente = new HttpClient())
                {
                    HttpResponseMessage response = await cliente.PostAsync(uri, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Si la solicitud no es exitosa, lanzar una excepción con el motivo del error
                        throw new Exception($"Error al obtener el usuario: {response.ReasonPhrase}");
                    }

                    // Si la autenticación es exitosa, redirigir al usuario al index
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante el proceso de autenticación
                // Si la excepción es debido a credenciales inválidas, se mostrará un mensaje adecuado en la vista
                ViewData["Mensaje"] = $"Error al iniciar sesión: {ex.Message}";
                return View();
            }
        }
    }
}