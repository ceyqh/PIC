using PIC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PIC.Model;
using System.Configuration;

namespace PIC.APIClient
{
    internal class PrestecsApiClient
    {
        string BaseUri;

        public PrestecsApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTS ELS PRESTECS
        public async Task<List<Prestec>> GetAllPrestecsAsync()
        {
            List<Prestec> prestec = new List<Prestec>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("prestecs");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        prestec = await response.Content.ReadAsAsync<List<Prestec>>();
                        response.Dispose();
                        return prestec;
                    }
                }
                
                // Si falla
                catch
                {
                    return null;
                }
            }
            return prestec;
        }

        // PRESTEC PER ID
        public async Task<Prestec> GetPrestecPerIdAsync(int Id)
        {
            Prestec prestec = new Prestec();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"prestecs/{Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        prestec = await response.Content.ReadAsAsync<Prestec>();
                        response.Dispose();
                        return prestec;
                    }
                }
                

                // Si falla
                catch
                {
                    return null;
                }
            }
            return prestec;
        }
    }
}
