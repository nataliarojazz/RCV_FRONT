using Newtonsoft.Json;
using RCV_FRONT.Models;
using System.Net.Http.Headers;
using System.Text;


namespace RCV_FRONT.Servicios
{
    public class Servicio_APIV : IServicio_API
    {

        private static string _documento;
        public static string _clave;
        private static string _baseurl;
        private static string _token;



        public Servicio_APIV()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            _baseurl = builder.GetSection("ApiSettings:baseUrl").Value;
        }

        public async Task Autenticar()
        {
            using (var cliente = new HttpClient { BaseAddress = new Uri(_baseurl) })
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


        public async Task<List<Vehiculo>> ListaV()
        {
            List<Vehiculo>? listaV = new();
          
            using (var cliente = new HttpClient())
            {
                string uri = _baseurl + "api/Vehiculo/ListaV";
                HttpResponseMessage result = await cliente.GetAsync(uri);
                if (result.IsSuccessStatusCode)
                {
                    string content = await result.Content.ReadAsStringAsync();
                    ResultadoApi resApi = JsonConvert.DeserializeObject<ResultadoApi>(content);
                    listaV = resApi.responseV;
                }
            }
            return listaV; // En caso de error, devuelve vacío
        }



        public async Task<Vehiculo> ObtenerV(int idVehiculo)
        {
            Vehiculo objeto = new Vehiculo();
            
          //  await Autenticar();
            
            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri(_baseurl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            var response = await cliente.GetAsync($"api/Vehiculo/obtenerV/{idVehiculo}");

            if (response.IsSuccessStatusCode)
            {
                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<ResultadoApi>(json_respuesta);
                objeto = resultado.objetoV;
            }
            return objeto; // En caso de error, devuelve vacío
        }

        public async Task<bool> GuardarV(Vehiculo objetoI)
        {
            bool respuesta = false;

          //  await Autenticar();

            var usuario = new HttpClient();
            usuario.BaseAddress = new Uri(_baseurl);
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var content = new StringContent(JsonConvert.SerializeObject(objetoI), Encoding.UTF8, "application/json");

            var response = await usuario.PostAsync("api/Vehiculo/GuardaV/", content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }
            return respuesta;
        }
        public async Task<bool> EditarV(Vehiculo objeto)
        {
            bool respuesta = false;

          //  await Autenticar();

            var usuario = new HttpClient();
            usuario.BaseAddress = new Uri(_baseurl);
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var content = new StringContent(JsonConvert.SerializeObject(objeto), Encoding.UTF8, "application/json");

            var response = await usuario.PutAsync("api/Vehiculo/EditarV/", content);

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }
            return respuesta;
        }

        public async Task<bool> EliminarV(int idVehiculo)
        {
            bool respuesta = false;

          //  await Autenticar();
            var usuario = new HttpClient();
            usuario.BaseAddress = new Uri(_baseurl);
            usuario.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await usuario.DeleteAsync($"api/Vehiculo/EliminarV/{idVehiculo}");

            if (response.IsSuccessStatusCode)
            {
                respuesta = true;
            }
            return respuesta;
        }

    }
}



