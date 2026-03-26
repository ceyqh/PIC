using PIC.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PIC.APIClient
{
    internal class AlumnesApiClient
    {
        string BaseUri;

        public AlumnesApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];
        }

        // AFEGIR ALUMNE
        public async Task<Alumne> PostAlumneAsync(Alumne nouAlumne)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Enviem una petició POST amb JSON directament
                HttpResponseMessage response = await client.PostAsJsonAsync("alumnes", nouAlumne);

                if (response.IsSuccessStatusCode)
                {
                    // Retornem l'usuari creat amb l'ID assignat pel servidor
                    var createdAlumne = await response.Content.ReadAsAsync<Alumne>();
                    response.Dispose();
                    return createdAlumne;
                }
                else
                {
                    throw new Exception($"Error al crear usuari: {response.StatusCode}");
                }
            }
        }

        // ACTUALITZAR ALUMNE
        public async Task<int> UpdateAlumneAsync(Alumne alumne)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PutAsJsonAsync($"alumnes/{alumne.IdUsuari}", alumne);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<int>();
                    return result;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error API ({response.StatusCode}): {errorContent}");
                }
            }
        }
    }
}
