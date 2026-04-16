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

            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
                throw new Exception("Error, no s'ha trobat la clau de la API");
            }
        }

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
    }
}
