using PIC.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIC.APIClient
{
    internal class ProfessorsApiClient
    {
        string BaseUri;

        public ProfessorsApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];
        }

        // AFEGIR PROFESSOR
        public async Task<Professor> PostProfessorAsync(Professor nouProfessor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Enviem una petició POST amb JSON directament
                HttpResponseMessage response = await client.PostAsJsonAsync("professors", nouProfessor);

                if (response.IsSuccessStatusCode)
                {
                    // Retornem l'usuari creat amb l'ID assignat pel servidor
                    var createdProfessor = await response.Content.ReadAsAsync<Professor>();
                    response.Dispose();
                    return createdProfessor;
                }
                else
                {
                    throw new Exception($"Error al crear usuari: {response.StatusCode}");
                }
            }
        }

        // ACTUALITZAR PROFESSOR
        public async Task<int> UpdateProfessorAsync(Professor professor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PutAsJsonAsync($"professors/{professor.IdUsuari}", professor);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<int>();
                    return result;
                }
                else
                {
                    // Això t'ajudarà a depurar si torna a fallar
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error API ({response.StatusCode}): {errorContent}");
                }
            }
        }

        // ESBORRAR PROFESSOR
        public async Task<int> DeleteProfessorAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"professors/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // Si el teu backend retorna algun valor (ex: 1)
                    var result = await response.Content.ReadAsAsync<int>();
                    response.Dispose();
                    return result;
                }
                else
                {
                    throw new Exception($"Error al esborrar el professor: {response.StatusCode}");
                }
            }
        }
    }
}
