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
    internal class CursosApiClient
    {
        string BaseUri;

        public CursosApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
                throw new Exception("Error, no s'ha trobat la clau de la API");
            }
        }

        // TOTS ELS CURSOS
        public async Task<List<Curs>> GetAllCursosAsync()
        {
            List<Curs> curs = new List<Curs>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /cursos}
                HttpResponseMessage response = await client.GetAsync("cursos");
                if (response.IsSuccessStatusCode)
                {
                    //Obtenim el resultat i el carreguem al objecte llista de cursos
                    curs = await response.Content.ReadAsAsync<List<Curs>>();
                    response.Dispose();
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? missatge?
                }
            }
            return curs;
        }

        // CURS PER ID
        public async Task<Curs> GetCursPerIdAsync(int Id)
        {
            Curs curs = new Curs();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /cursos/{Id}
                HttpResponseMessage response = await client.GetAsync($"cursos/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        curs = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte Curs
                        curs = await response.Content.ReadAsAsync<Curs>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return curs;
        }

        // AFEGIR CURS
        public async Task<Curs> PostCursAsync(Curs nouCurs)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Enviem una petició POST amb JSON directament
                HttpResponseMessage response = await client.PostAsJsonAsync("cursos", nouCurs);

                if (response.IsSuccessStatusCode)
                {
                    // Retornem el curs creat amb l'ID assignat pel servidor
                    var createdCurs = await response.Content.ReadAsAsync<Curs>();
                    response.Dispose();
                    return createdCurs;
                }
                else
                {
                    throw new Exception("Error al crear el curs: "+ response.StatusCode);
                }
            }
        }

        // ACTUALITZAR CURS
        public async Task<int> UpdateCursAsync(Curs curs)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PutAsJsonAsync($"cursos/{curs.Id}", curs);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<int>();
                    return result;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception("Error al actualitzar el curs " + response.StatusCode);
                }
            }
        }

        // ESBORRAR CURS
        public async Task<int> DeleteCursAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"cursos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Si el teu backend retorna algun valor (ex: 1)
                    var result = await response.Content.ReadAsAsync<int>();
                    response.Dispose();
                    return result;
                }
                else
                {
                    throw new Exception($"Error al esborrar el curs: {response.StatusCode}");
                }
            }
        }
    }
}
