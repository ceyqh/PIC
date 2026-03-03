using PIC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PIC.Model;
using System.Configuration;

namespace PIC.APIClient
{
    internal class DispositiusApiClient
    {
        string BaseUri;

        public DispositiusApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];
        }

        public async Task<Dispositiu> GetDispositiuPerIdAsync(int Id)
        {
            Dispositiu dispositiu = new Dispositiu();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /dispositius/{Id}
                HttpResponseMessage response = await client.GetAsync($"dispositius/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        dispositiu = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte dispositius
                        dispositiu = await response.Content.ReadAsAsync<Dispositiu>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return dispositiu;
        }

        public async Task<List<Dispositiu>> GetAllDispositiusAsync()
        {
            List<Dispositiu> dispositiu = new List<Dispositiu>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /dispositius}
                HttpResponseMessage response = await client.GetAsync("dispositius");
                if (response.IsSuccessStatusCode)
                {
                    //Obtenim el resultat i el carreguem al objecte llista de dispositius
                    dispositiu = await response.Content.ReadAsAsync<List<Dispositiu>>();
                    response.Dispose();
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? missatge?
                }
            }
            return dispositiu;
        }

        public async Task AddDispositiuAsync(Dispositiu dispositiu)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició POST al endpoint /dispositius}
                HttpResponseMessage response = await client.PostAsJsonAsync("dispositius", dispositiu);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task UpdateDispositiuAsync(Dispositiu dispositiu)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició PUT al endpoint /users/Id
                HttpResponseMessage response = await client.PutAsJsonAsync($"dispositius/{dispositiu.Id}", dispositiu);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task DeleteDispositiuAsync(int Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició DELETE al endpoint /users/Id
                HttpResponseMessage response = await client.DeleteAsync($"dispositius/{Id}");
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
