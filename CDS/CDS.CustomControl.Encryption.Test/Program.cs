using CDS.CustomControl.Encryption.Test.Authentication;
using System.Configuration;
using System;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;

namespace CDS.CustomControl.Encryption.Test
{
    class Program
    {
        const string azureFunctionUrl = "https://cdscryptography.azurewebsites.net/api/HttpTrigger1";
        static readonly HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(azureFunctionUrl);
        static readonly HttpClient httpClient = new HttpClient();

        static void Main(string[] args)
        {
            Console.WriteLine("Main because why not");
            PowerPlatformEnvironment environment = new PowerPlatformEnvironment(
                ConfigurationSettings.AppSettings["server"],
                ConfigurationSettings.AppSettings["user"],
                ConfigurationSettings.AppSettings["password"],
                "fire", "cap");

            PowerApp powerApp = new PowerApp(environment);

            // powerApp.GetSolutions();

            CallExternaelAuthService();

            Console.WriteLine("Nook");            
        }        

        static async void CallExternaelAuthService()
        {
            // one
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", AppConfig.CLIENT_ID),
                new KeyValuePair<string, string>("client_secret", AppConfig.CLIENT_SECRET),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", AppConfig.EXTERNEL_USER),
                new KeyValuePair<string, string>("password", AppConfig.EXTERNEL_PASSWORD),
                new KeyValuePair<string, string>("scope", "library+reporting+download")
            });            
            try
            {
                var response = new HttpResponseMessage();
                response = await httpClient.PostAsync("https://auth.seismic.com/tenants/tstrader/connect/token", formUrlEncodedContent);
                //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                //{
                //    var responseText = streamReader.ReadToEnd();
                //    Console.WriteLine(responseText);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Nook");

            // two
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://auth.seismic.com/tenants/tstrader/connect/token");
            //httpWebRequest.ContentType = "application/json; charset=utf-8";
            //httpWebRequest.Method = "POST";
            //string json = "";
            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    json = "{ \"data\" : \"client_id=" + AppConfig.CLIENT_ID 
            //        + "&client_secret=" + AppConfig.CLIENT_SECRET 
            //        + "&grant_type=password" 
            //        + "&username=" + AppConfig.EXTERNEL_USER 
            //        + "&password=" + AppConfig.EXTERNEL_PASSWORD 
            //        + "&scope=library+reporting+download\"}";
            //    streamWriter.Write(json);
            //    streamWriter.Flush();
            //}

            //try
            //{
            //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //    {
            //        var responseText = streamReader.ReadToEnd();
            //        Console.WriteLine(responseText);
            //    }
            //}
            //catch (WebException ex)
            //{

            //    Console.WriteLine(ex.Message);
            //    throw;
            //}


            Console.WriteLine("Nook");
        }

        static void CallAzureFunction()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(azureFunctionUrl);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            string json = "";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                json = "{ \"name\" : \"Isabelle\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine(responseText);
                }
            }
            catch (WebException ex)
            {

                Console.WriteLine(ex.Message);
                throw;
            }
            Console.WriteLine("Nook");
        }
    }
}
