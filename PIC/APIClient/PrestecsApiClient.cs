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
                        return prestec;
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
                        return prestec;
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

        // PRÉSTECS PER USUARI
        public async Task<List<Prestec>> GetPrestecsPerIdUsuariAsync(int Id)
        {
            List<Prestec> prestecs = new List<Prestec>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"prestecs/usuaris?id={Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        prestecs = await response.Content.ReadAsAsync<List<Prestec>>();
                        return prestecs;
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

        // PRÉSTECS PER USUARI
        public async Task<List<Prestec>> GetPrestecsPerIdDispositiuAsync(int Id)
        {
            List<Prestec> prestecs = new List<Prestec>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"prestecs/dispositius?id={Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        prestecs = await response.Content.ReadAsAsync<List<Prestec>>();
                        return prestecs;
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

        // AFEGIR PRÉSTEC
        public async Task<Prestec> PostPrestecAsync(Prestec nouPrestec)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("prestecs", nouPrestec);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdPrestec = await response.Content.ReadAsAsync<Prestec>(); 
                        return createdPrestec;
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

        // ACTUALITZAR PRÉSTEC
        public async Task<int> UpdatePrestecAsync(Prestec prestec)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"prestecs/{prestec.Id}", prestec);
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

        // ESBORRAR PRÉSTEC
        public async Task<int> DeletePrestecAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"prestecs/{id}");
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
