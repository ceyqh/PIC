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

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync("departaments");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        departament = await response.Content.ReadAsAsync<List<Departament>>();
                        response.Dispose();
                        return departament;
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

        // DEPARTAMENT PER ID
        public async Task<Departament> GetDepartamentPerIdAsync(int Id)
        {
            Departament departament = new Departament();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"departaments/{Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        departament = await response.Content.ReadAsAsync<Departament>();
                        response.Dispose();
                        return departament;
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

        // AFEGIR DEPARTAMENT
        public async Task<Departament> PostDepartamentAsync(Departament nouDepartament)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("departaments", nouDepartament);
                    if (response.IsSuccessStatusCode)
                    {
                        // Retorn
                        var createdCurs = await response.Content.ReadAsAsync<Departament>();
                        response.Dispose();
                        return createdCurs;
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

        // ACTUALITZAR DEPARTAMENT
        public async Task<int> UpdateDepartamentAsync(Departament departament)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"departaments/{departament.Id}", departament);
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

        // ESBORRAR DEPARTAMENT
        public async Task<int> DeleteDepartamentAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Petició
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync($"departaments/{id}");
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
