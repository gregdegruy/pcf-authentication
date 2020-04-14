using CDS.CustomControl.Encryption.Test.Authentication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using Xunit;

namespace CDS.CustomControl.Encryption.Test
{
    public class IntegrationTest
    {
        const string azureFunctionUrl = "https://cdscryptography.azurewebsites.net/api/HttpTrigger1";
        static readonly HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(azureFunctionUrl);
        static readonly HttpClient httpClient = new HttpClient();

        [Fact]
        public void CDSAuthentication()
        {
            PowerPlatformEnvironment environment = new PowerPlatformEnvironment(
                ConfigurationSettings.AppSettings["server"],
                ConfigurationSettings.AppSettings["user"],
                ConfigurationSettings.AppSettings["password"],
                "fire", "cap");

            PowerApp powerApp = new PowerApp(environment);

            // Breaks, can not auth in CDS over xunit for some reason
            // powerApp.GetSolutions();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetResponseFromAzureFunctionAsync()
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

            try {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine(responseText);
                }
            } catch (WebException ex) {

                Console.WriteLine(ex.Message);
                throw;
            }
            Console.WriteLine("Nook");
        }

        [Fact]
        public async System.Threading.Tasks.Task CallExternalAuthService()
        {
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", AppConfig.CLIENT_ID),
                new KeyValuePair<string, string>("client_secret", AppConfig.CLIENT_SECRET),
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("SHA", AppConfig.EXTERNEL_USER),
                new KeyValuePair<string, string>("SHA", AppConfig.EXTERNEL_PASSWORD),
                new KeyValuePair<string, string>("scope", "library+reporting+download")
            });
            var response = new HttpResponseMessage();
            response = await httpClient.PostAsync("https://auth.seismic.com/tenants/tstrader/connect/token", formUrlEncodedContent);
            Console.WriteLine("Nook");
        }
    }
}
