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
                throw new Exception("Error, no s'ha trobat la clau de la API");
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

                //Enviem una petició GET al endpoint /cursos}
                HttpResponseMessage response = await client.GetAsync("categories");
                if (response.IsSuccessStatusCode)
                {
                    //Obtenim el resultat i el carreguem al objecte llista de cursos
                    categoria = await response.Content.ReadAsAsync<List<Categoria>>();
                    response.Dispose();
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? missatge?
                }
            }
            return categoria;
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

                //Enviem una petició GET al endpoint /cursos/{Id}
                HttpResponseMessage response = await client.GetAsync($"categories/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        categoria = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte Curs
                        categoria = await response.Content.ReadAsAsync<Categoria>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return categoria;
        }

        // AFEGIR CATEGORIA
        public async Task<Categoria> PostCategoriaAsync(Categoria nouCategoria)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Enviem una petició POST amb JSON directament
                HttpResponseMessage response = await client.PostAsJsonAsync("categories", nouCategoria);

                if (response.IsSuccessStatusCode)
                {
                    // Retornem el curs creat amb l'ID assignat pel servidor
                    var createdCategoria = await response.Content.ReadAsAsync<Categoria>();
                    response.Dispose();
                    return createdCategoria;
                }
                else
                {
                    throw new Exception("Error al crear la categoria: " + response.StatusCode);
                }
            }
        }

        // ACTUALITZAR CURS
        public async Task<int> UpdateCategoriaAsync(Categoria categoria)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PutAsJsonAsync($"categories/{categoria.Id}", categoria);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<int>();
                    return result;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception("Error al actualitzar la categoria " + response.StatusCode);
                }
            }
        }

        // ESBORRAR CURS
        public async Task<int> DeleteCategoriaAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Si el teu backend retorna algun valor (ex: 1)
                    var result = await response.Content.ReadAsAsync<int>();
                    response.Dispose();
                    return result;
                }
                else
                {
                    throw new Exception($"Error al esborrar la categoria: {response.StatusCode}");
                }
            }
        }
    }
}
