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
    internal class PrestecsApiClient
    {
        string BaseUri;

        public PrestecsApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];

            // Si no troba la ruta de l'API
            if (string.IsNullOrEmpty(BaseUri))
            {
                BaseUri = "http://localhost/temp";
            }
        }

        public async Task<List<Prestec>> GetAllPrestecsAsync()
        {
            List<Prestec> prestec = new List<Prestec>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /prestecs}
                HttpResponseMessage response = await client.GetAsync("prestecs-info");
                if (response.IsSuccessStatusCode)
                {
                    //Obtenim el resultat i el carreguem al objecte llista de prestecs
                    prestec = await response.Content.ReadAsAsync<List<Prestec>>();
                    response.Dispose();
                }
                else
                {
                    return null;
                }
            }
            return prestec;
        }

        public async Task<Prestec> GetPrestecPerIdAsync(int Id)
        {
            Prestec prestec = new Prestec();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /prestecs/{Id}
                HttpResponseMessage response = await client.GetAsync($"prestecs-info/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        prestec = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte Prestec
                        prestec = await response.Content.ReadAsAsync<Prestec>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return prestec;
        }

        public async Task AddPrestecAsync(Prestec prestec)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició POST al endpoint /prestecs}
                HttpResponseMessage response = await client.PostAsJsonAsync("prestecs-info", prestec);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task UpdatePrestecAsync(Prestec prestec)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició PUT al endpoint /users/Id
                HttpResponseMessage response = await client.PutAsJsonAsync($"prestecs-info/{prestec.Id}", prestec);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task DeletePrestecAsync(int Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició DELETE al endpoint /users/Id
                HttpResponseMessage response = await client.DeleteAsync($"prestecs-info/{Id}");
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
