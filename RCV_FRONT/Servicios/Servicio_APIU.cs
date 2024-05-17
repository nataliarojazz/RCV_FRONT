using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RCV_FRONT.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace RCV_FRONT.Servicios
{
    public class Servicio_APIU : IServicioU_API
    {

        private static string _documento;
        public static string _clave;
        private static string _baseurl;
        private static string _token;


        public Servicio_APIU()
        {

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            _baseurl = builder.GetSection("ApiSettings:baseUrl").Value;
        }
        public async Task Autenticar()
        {
            using (var cliente = new HttpClient())
            {
                var credenciales = new Credencial() { Documento = _documento, Clave = _clave };
                var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");
                var response = await cliente.PostAsync("api/Autenticacion/Validar", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Si la solicitud no es exitosa, lanzar una excepción con el motivo de la falta de autorización
                    throw new Exception($"Error en la autenticación: {response.ReasonPhrase}");
                }

                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<ResultadoCredencial>(json_respuesta);

                _token = resultado.token;
            }
        }
        public async Task<List<Usuario>> ListaU()
        {
            List<Usuario>? listaU = new();
           // await Autenticar();

            using (var cliente = new HttpClient())
            {
                string uri = _baseurl + "api/Usuario/ListaU";
                HttpResponseMessage result = await cliente.GetAsync(uri);
                if (result.IsSuccessStatusCode)
                {
                    string content = await result.Content.ReadAsStringAsync();
                    ResultadoApi resApi = JsonConvert.DeserializeObject<ResultadoApi>(content);
                    listaU = resApi.response;
                }
            }
            return listaU; // En caso de error, devuelve vacío
        }

        public async Task<Usuario> ObtenerU(int idUusario)
        {
            Usuario objeto = new Usuario();

            //await Autenticar();

            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseurl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await cliente.GetAsync($"api/Usuario/obtenerU/{idUusario}");

            if (response.IsSuccessStatusCode)
            {
                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<ResultadoApi>(json_respuesta);
                objeto = resultado.objetoU;
            }
            return objeto; // En caso de error, devuelve vacío
        }

        public async Task<bool> GuardarU(Usuario objetoU)
        {
            bool respuesta = false;

            //await Autenticar();

            var usuario = new HttpClient();
            usuario.BaseAddress = new Uri(_baseurl);
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var content = new StringContent(JsonConvert.SerializeObject(objetoU), Encoding.UTF8, "application/json");

            var response = await usuario.PostAsync("api/Uusario/GuardaU/", content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }
            return respuesta;
        }
        public async Task<bool> EditarU(Usuario objeto)
        {
            bool respuesta = false;

            //await Autenticar();

            var usuario = new HttpClient();
            usuario.BaseAddress = new Uri(_baseurl);
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var content = new StringContent(JsonConvert.SerializeObject(objeto), Encoding.UTF8, "application/json");

            var response = await usuario.PutAsync("api/Usuario/EditarU/", content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }
            return respuesta;
        }

        public async Task<bool> EliminarU(int idUsuario)
        {
            bool respuesta = false;

           // await Autenticar();
            var usuario = new HttpClient();
            usuario.BaseAddress = new Uri(_baseurl);
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await usuario.DeleteAsync($"api/Usuario/EliminarU/{idUsuario}");

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }
            return respuesta;
        }
        public async Task<Usuario> GetUsuario(string documento, string clave)
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

                    // Leer y deserializar la respuesta JSON en un objeto Usuario
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Usuario usuario = JsonConvert.DeserializeObject<Usuario>(responseContent);

                    return usuario;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante el proceso de autenticación
                throw new Exception($"Error al obtener el usuario: {ex.Message}");
            }
        }
    }
}