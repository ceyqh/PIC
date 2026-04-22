using PIC.Model;
using PIC.View;
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
    internal class DepartamentsApiClient
    {
        string BaseUri;

        public DepartamentsApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
                throw new Exception("Error, no s'ha trobat la clau de la API");
            }
        }

        // TOTS ELS DEPARTAMENTS
        public async Task<List<Departament>> GetAllDepartamentsAsync()
        {
            List<Departament> departament = new List<Departament>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /departaments}
                HttpResponseMessage response = await client.GetAsync("departaments");
                if (response.IsSuccessStatusCode)
                {
                    //Obtenim el resultat i el carreguem al objecte llista de departaments
                    departament = await response.Content.ReadAsAsync<List<Departament>>();
                    response.Dispose();
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? missatge?
                }
            }
            return departament;
        }

        // DEPARTAMENT PER ID
        public async Task<Departament> GetDepartamentPerIdAsync(int Id)
        {
            Departament departament = new Departament();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /cursos/{Id}
                HttpResponseMessage response = await client.GetAsync($"departaments/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        departament = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte Curs
                        departament = await response.Content.ReadAsAsync<Departament>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return departament;
        }

        // AFEGIR DEPARTAMENT
        public async Task<Departament> PostDepartamentAsync(Departament nouDepartament)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Enviem una petició POST amb JSON directament
                HttpResponseMessage response = await client.PostAsJsonAsync("departaments", nouDepartament);

                if (response.IsSuccessStatusCode)
                {
                    // Retornem el curs creat amb l'ID assignat pel servidor
                    var createdCurs = await response.Content.ReadAsAsync<Departament>();
                    response.Dispose();
                    return createdCurs;
                }
                else
                {
                    throw new Exception("Error al crear el departament: " + response.StatusCode);
                }
            }
        }

        // ACTUALITZAR DEPARTAMENT
        public async Task<int> UpdateDepartamentAsync(Departament departament)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PutAsJsonAsync($"departaments/{departament.Id}", departament);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<int>();
                    return result;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception("Error al actualitzar el departament " + response.StatusCode);
                }
            }
        }

        // ESBORRAR DEPARTAMENT
        public async Task<int> DeleteDepartamentAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"departaments/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Si el teu backend retorna algun valor (ex: 1)
                    var result = await response.Content.ReadAsAsync<int>();
                    response.Dispose();
                    return result;
                }
                else
                {
                    throw new Exception($"Error al esborrar el departament: {response.StatusCode}");
                }
            }
        }
    }
}
