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

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        // AFEGIR PROFESSOR
        public async Task<Professor> PostProfessorAsync(Professor nouProfessor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("professors", nouProfessor);

                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdProfessor = await response.Content.ReadAsAsync<Professor>();
                        response.Dispose();
                        return createdProfessor;
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

        // ACTUALITZAR PROFESSOR
        public async Task<int> UpdateProfessorAsync(Professor professor)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"professors/{professor.IdUsuari}", professor);

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

        // ESBORRAR PROFESSOR
        public async Task<int> DeleteProfessorAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"professors/{id}");

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
