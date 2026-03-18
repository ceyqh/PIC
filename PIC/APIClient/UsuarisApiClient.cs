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

        // TOTS ELS USUARIS
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

        // USUARI PER ID
        public async Task<Usuari> GetUsuariPerIdAsync(int Id)
        {
            Usuari usuari = new Usuari();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /usuaris/{Id}
                HttpResponseMessage response = await client.GetAsync($"usuaris/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        usuari = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte Usuari
                        usuari = await response.Content.ReadAsAsync<Usuari>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return usuari;
        }

        // USUARIS PER CURS
        public async Task<List<Usuari>> GetUsuarisPerIdCursAsync(int Id)
        {
            List<Usuari> usuari = new List<Usuari>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /usuaris/cursos?id={Id}
                HttpResponseMessage response = await client.GetAsync($"usuaris/cursos?id={Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        usuari = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte Usuari
                        usuari = await response.Content.ReadAsAsync<List<Usuari>>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return usuari;
        }

        // USUARIS PER DEPARTAMENT
        public async Task<List<Usuari>> GetUsuarisPerIdDepartamentAsync(int Id)
        {
            List<Usuari> usuari = new List<Usuari>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Enviem una petició GET al endpoint /usuaris/departaments?id={Id}
                HttpResponseMessage response = await client.GetAsync($"usuaris/departaments?id={Id}");
                if (response.IsSuccessStatusCode)
                {
                    //Reposta 204 quan no ha trobat dades
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        usuari = null;
                    }
                    else
                    {
                        //Obtenim el resultat i el carreguem al Objecte Usuari
                        usuari = await response.Content.ReadAsAsync<List<Usuari>>();
                        response.Dispose();
                    }
                }
                else
                {
                    //TODO: que fer si ha anat malament? retornar null? 
                }
            }
            return usuari;
        }

        // AFEGIR USUARI
        public async Task<Usuari> PostUsuariAsync(Usuari newUser)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Enviem una petició POST amb JSON directament
                HttpResponseMessage response = await client.PostAsJsonAsync("usuaris", newUser);

                if (response.IsSuccessStatusCode)
                {
                    // Retornem l'usuari creat amb l'ID assignat pel servidor
                    var createdUser = await response.Content.ReadAsAsync<Usuari>();
                    response.Dispose();
                    return createdUser;
                }
                else
                {
                    throw new Exception($"Error al crear usuari: {response.StatusCode}");
                }
            }
        }

        // ACTUALITZAR USUARI
        public async Task<int> UpdateUsuariAsync(Usuari user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Enviem una petició PUT a /usuaris/{id} amb l'usuari
                HttpResponseMessage response = await client.PutAsJsonAsync($"usuaris/{user.Id}", user);

                if (response.IsSuccessStatusCode)
                {
                    // Retorna el resultat del servidor (normalment 1 si tot bé)
                    var result = await response.Content.ReadAsAsync<int>();
                    response.Dispose();
                    return result;
                }
                else
                {
                    throw new Exception($"Error al actualitzar l'usuari: {response.StatusCode}");
                }
            }
        }
    }
}
