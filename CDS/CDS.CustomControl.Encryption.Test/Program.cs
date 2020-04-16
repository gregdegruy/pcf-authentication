using CDS.CustomControl.Encryption.Test.Authentication;
using System.Configuration;
using System;
using System.Net;
using System.IO;
using RestSharp;

namespace CDS.CustomControl.Encryption.Test
{
    class Program
    {     
        static void Main(string[] args)
        {
            Console.WriteLine("Main because why not");
            PowerPlatformEnvironment environment = new PowerPlatformEnvironment(
                ConfigurationSettings.AppSettings["server"],
                ConfigurationSettings.AppSettings["user"],
                ConfigurationSettings.AppSettings["password"],
                "fire", "cap");
            PowerApp powerApp = new PowerApp(environment);

            CallAzureFunction();

            Console.WriteLine("Done");
        }

        static void CallAzureFunction()
        {
            const string azureFunctionUrl = "https://cdscryptography.azurewebsites.net/api/HttpTriggerRestSharp";
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

        static void TestRestSharpLocally()
        {
            var client = new RestClient("https://auth.seismic.com/tenants/tstrader/connect/token ");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", AppConfig.CLIENT_ID);
            request.AddParameter("client_secret", AppConfig.CLIENT_SECRET);
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", AppConfig.EXTERNEL_USER);
            request.AddParameter("password", AppConfig.EXTERNEL_PASSWORD);
            request.AddParameter("scope", "library reporting download");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
