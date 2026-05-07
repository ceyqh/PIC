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
    internal class UsuarisBaseApiClient
    {
        string BaseUri;

        public UsuarisBaseApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTS ELS USUARIS BASE
        public async Task<List<UsuariBase>> GetAllUsuarisAsync()
        {
            List<UsuariBase> usuari = new List<UsuariBase>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("usuaris-base");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        usuari = await response.Content.ReadAsAsync<List<UsuariBase>>();
                        return usuari;
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

        // ESBORRAR USUARI BASE
        public async Task<int> DeleteUsuariAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"usuaris-base/{id}");
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
