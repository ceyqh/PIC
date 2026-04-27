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
    internal class AdministradorsApiClient
    {
        string BaseUri;

        public AdministradorsApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTS ELS ADMINISTRADORS
        public async Task<List<Administrador>> GetAllAdministradorsAsync()
        {
            List<Administrador> administrador = new List<Administrador>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("administradors");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        administrador = await response.Content.ReadAsAsync<List<Administrador>>();
                        return administrador;
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

        // AFEGIR ADMINISTRADOR
        public async Task<Administrador> PostAlumneAsync(Administrador nouAdministrador)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("administradors", nouAdministrador);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdAlumne = await response.Content.ReadAsAsync<Administrador>();
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
    }
}
