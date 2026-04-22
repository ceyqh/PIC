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
    internal class UsuarisApiClient
    {
        string BaseUri;

        public UsuarisApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTS ELS USUARIS
        public async Task<List<Usuari>> GetAllUsuarisAsync()
        {
            List<Usuari> usuari = new List<Usuari>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("usuaris");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        usuari = await response.Content.ReadAsAsync<List<Usuari>>();
                        response.Dispose();
                        return usuari;
                    }
                }
                
                // Si falla
                catch
                {
                    return null;
                }
            }
            return usuari;
        }

        // USUARI PER ID
        public async Task<Usuari> GetUsuariPerIdAsync(int Id)
        {
            Usuari usuari = new Usuari();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"usuaris/{Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        usuari = await response.Content.ReadAsAsync<Usuari>();
                        response.Dispose();
                        return usuari;
                    }
                }
                

                // Si falla
                catch
                {
                    return null;
                }
            }
            return usuari;
        }

        // USUARIS PER CURS
        public async Task<List<Usuari>> GetUsuarisPerIdCursAsync(int Id)
        {
            List<Usuari> usuari = new List<Usuari>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"usuaris/cursos?id={Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        usuari = await response.Content.ReadAsAsync<List<Usuari>>();
                        response.Dispose();
                        return usuari;
                    }
                }
                
                // Si falla
                catch
                {
                    return null;
                }
            }
            return usuari;
        }

        // USUARIS PER DEPARTAMENT
        public async Task<List<Usuari>> GetUsuarisPerIdDepartamentAsync(int Id)
        {
            List<Usuari> usuari = new List<Usuari>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"usuaris/departaments?id={Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        usuari = await response.Content.ReadAsAsync<List<Usuari>>();
                        response.Dispose();
                        return usuari;
                    }
                }
                
                // Si falla
                catch
                {
                    return null;
                }
            }
            return usuari;
        }

        // AFEGIR USUARI
        public async Task<NouUsuari> PostUsuariAsync(NouUsuari nouUsuari)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("usuaris", nouUsuari);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdUser = await response.Content.ReadAsAsync<NouUsuari>();
                        response.Dispose();
                        return createdUser;
                    }
                }
                
                // Si falla
                catch
                {
                    return null;
                }
            }
            return nouUsuari;
        }

        // ACTUALITZAR USUARI
        public async Task<int> UpdateUsuariAsync(Usuari usuari)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"usuaris/{usuari.Id}", usuari);
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

        // ESBORRAR USUARI
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
                    HttpResponseMessage response = await client.DeleteAsync($"usuaris/{id}");
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
