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
    internal class RegistresApiClient
    {
        string BaseUri;

        public RegistresApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTS ELS REGISTRES
        public async Task<List<Registre>> GetAllRegistresAsync()
        {
            List<Registre> registre = new List<Registre>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("registres");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        registre = await response.Content.ReadAsAsync<List<Registre>>(); 
                        return registre;
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

        // REGISTRES PER ID PRÉSTEC
        public async Task<List<Registre>> GetRegistresPerIdPrestecAsync(int Id)
        {
            List<Registre> registres = new List<Registre>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"registres/prestec?id={Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        registres = await response.Content.ReadAsAsync<List<Registre>>(); 
                        return registres;
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

        // REGISTRES PER ID DISPOSITIU
        public async Task<List<Registre>> GetRegistresPerIdDispositiuAsync(int Id)
        {
            List<Registre> registres = new List<Registre>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"registres/dispositiu?id={Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        registres = await response.Content.ReadAsAsync<List<Registre>>(); 
                        return registres;
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

        // REGISTRES PER NOM GRUP
        public async Task<List<Registre>> GetRegistresPerNomGrupAsync(string Nom)
        {
            List<Registre> registres = new List<Registre>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"registres/grup?id={Nom}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        registres = await response.Content.ReadAsAsync<List<Registre>>(); 
                        return registres;
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

        // AFEGIR REGISTRE
        public async Task<Registre> PostRegistreAsync(Registre nouRegistre)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("registres", nouRegistre);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdRegistre = await response.Content.ReadAsAsync<Registre>(); 
                        return createdRegistre;
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

        // ACTUALITZAR REGISTRE
        public async Task<int> UpdateRegistreAsync(Registre registre)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"registres/{registre.Id}", registre);
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
