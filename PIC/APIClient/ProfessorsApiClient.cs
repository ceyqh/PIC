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
    internal class ProfessorsApiClient
    {
        string BaseUri;

        public ProfessorsApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];
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
    }
}
