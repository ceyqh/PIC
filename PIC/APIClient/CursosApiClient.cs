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
        }

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
    }
}
