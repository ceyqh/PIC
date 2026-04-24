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
    internal class DispositiusApiClient
    {
        string BaseUri;

        public DispositiusApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTS ELS DISPOSITIUS 
        public async Task<List<Dispositiu>> GetAllDispositiusAsync()
        {
            List<Dispositiu> dispositiu = new List<Dispositiu>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("dispositius");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        dispositiu = await response.Content.ReadAsAsync<List<Dispositiu>>();
                        response.Dispose();
                        return dispositiu;
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

        // DISPOSITIU PER ID
        public async Task<Dispositiu> GetDispositiuPerIdAsync(int Id)
        {
            Dispositiu dispositiu = new Dispositiu();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"dispositius/{Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        dispositiu = await response.Content.ReadAsAsync<Dispositiu>();
                        response.Dispose();
                        return dispositiu;
                    }
                }
                
                catch
                {
                    return null; 
                }
            }
            return null;
        }

        // DISPOSITIUS PER CATEGORIA
        public async Task<List<Dispositiu>> GetDispositiusPerIdCategoriaAsync(int Id)
        {
            List<Dispositiu> dispositius = new List<Dispositiu>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"dispositius/categories?id={Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        dispositius = await response.Content.ReadAsAsync<List<Dispositiu>>();
                        response.Dispose();
                        return dispositius;
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

        // DISPOSITIUS DISPONIBLES
        public async Task<List<Dispositiu>> GetDispositiusDisponiblesAsync()
        {
            List<Dispositiu> dispositius = new List<Dispositiu>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"dispositius/disponibles");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        dispositius = await response.Content.ReadAsAsync<List<Dispositiu>>();
                        response.Dispose();
                        return dispositius;
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

        // DISPOSITIUS NO DISPONIBLES
        public async Task<List<Dispositiu>> GetDispositiusNoDisponiblesAsync()
        {
            List<Dispositiu> dispositius = new List<Dispositiu>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"dispositius/no-disponibles");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        dispositius = await response.Content.ReadAsAsync<List<Dispositiu>>();
                        response.Dispose();
                        return dispositius;
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

        // AFEGIR DISPOSITIU
        public async Task<NouDispositiu> PostDispositiuAsync(NouDispositiu nouDispositiu)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("dispositius", nouDispositiu);

                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var dispositiuCreat = await response.Content.ReadAsAsync<NouDispositiu>();
                        response.Dispose();
                        return dispositiuCreat;
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

        // ACTUALITZAR DISPOSITIU
        public async Task<int> UpdateDispositiuAsync(Dispositiu dispositiu)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try 
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"dispositius/{dispositiu.Id}", dispositiu);

                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var result = await response.Content.ReadAsAsync<int>();
                        response.Dispose();
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

        // ESBORRAR CURS
        public async Task<int> DeleteDispositiuAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"dispositius/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var result = await response.Content.ReadAsAsync<int>();
                        response.Dispose();
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
