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

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTS ELS ALUMNES
        public async Task<List<Alumne>> GetAllAlumnesAsync()
        {
            List<Alumne> alumnes = new List<Alumne>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("alumnes");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        alumnes = await response.Content.ReadAsAsync<List<Alumne>>();
                        return alumnes;
                    }
                }

                // Si falla
                catch
                {
                    return null;
                }
            }
            return null;
        }

        // AFEGIR ALUMNE
        public async Task<Alumne> PostAlumneAsync(Alumne nouAlumne)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("alumnes", nouAlumne);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdAlumne = await response.Content.ReadAsAsync<Alumne>();
                        return createdAlumne;
                    }
                }

                // Si falla
                catch
                {
                    return null;
                }
            }
            return null;
        }

        // ACTUALITZAR ALUMNE
        public async Task<int> UpdateAlumneAsync(Alumne alumne)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"alumnes/{alumne.IdUsuari}", alumne);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var result = await response.Content.ReadAsAsync<int>();
                        return result;
                    }
                }

                // Si falla
                catch
                {
                    return -1;
                }
            }
            return -1;
        }

        // ESBORRAR ALUMNE
        public async Task<int> DeleteAlumneAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                // Petició
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"alumnes/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var result = await response.Content.ReadAsAsync<int>();
                        return result;
                    }
                }
                
                // Si falla
                catch
                {
                    return -1;
                }
            }
            return -1;
        }
    }
}
