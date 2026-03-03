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
        }

        public async Task<List<Usuari>> GetAllUsuarisAsync()
        {
            List<Usuari> usuari = new List<Usuari>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /usuaris}
                HttpResponseMessage response = await client.GetAsync("usuaris");
                if (response.IsSuccessStatusCode)
                {
                    //Obtenim el resultat i el carreguem al objecte llista de usuaris
                    usuari = await response.Content.ReadAsAsync<List<Usuari>>();
                    response.Dispose();
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? missatge?
                }
            }
            return usuari;
        }
    }
}
