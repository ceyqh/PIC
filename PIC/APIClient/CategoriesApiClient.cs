using PIC.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PIC.APIClient
{
    internal class CategoriesApiClient
    {
        string BaseUri;

        public CategoriesApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // TOTES LES CATEGORIES
        public async Task<List<Categoria>> GetAllCategoriesAsync()
        {
            List<Categoria> categoria = new List<Categoria>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("categories");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        categoria = await response.Content.ReadAsAsync<List<Categoria>>();
                        response.Dispose();
                        return categoria;
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

        // CATEGORIA PER ID
        public async Task<Categoria> GetCategoriaPerIdAsync(int Id)
        {
            Categoria categoria = new Categoria();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"categories/{Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        categoria = await response.Content.ReadAsAsync<Categoria>();
                        response.Dispose();
                        return categoria;
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

        // AFEGIR CATEGORIA
        public async Task<Categoria> PostCategoriaAsync(Categoria novaCategoria)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("categories", novaCategoria);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdCategoria = await response.Content.ReadAsAsync<Categoria>();
                        response.Dispose();
                        return createdCategoria;
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

        // ACTUALITZAR CURS
        public async Task<int> UpdateCategoriaAsync(Categoria categoria)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"categories/{categoria.Id}", categoria);
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
        public async Task<int> DeleteCategoriaAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"categories/{id}");

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
