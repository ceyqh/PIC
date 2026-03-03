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
    internal class CategoriesApiClient
    {
        string BaseUri;

        public CategoriesApiClient()
        {
            BaseUri = ConfigurationManager.AppSettings["BaseUri"];
        }

        public async Task<List<Categoria>> GetAllCategoriesAsync()
        {
            List<Categoria> categoria = new List<Categoria>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /categories}
                HttpResponseMessage response = await client.GetAsync("categories");
                if (response.IsSuccessStatusCode)
                {
                    //Obtenim el resultat i el carreguem al objecte llista de categories
                    categoria = await response.Content.ReadAsAsync<List<Categoria>>();
                    response.Dispose();
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? missatge?
                }
            }
            return categoria;
        }
    }
}
