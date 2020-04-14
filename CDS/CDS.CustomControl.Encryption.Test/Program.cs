using CDS.CustomControl.Encryption.Test.Authentication;
using System.Configuration;
using System;
using System.Net;
using System.IO;

namespace CDS.CustomControl.Encryption.Test
{
    class Program
    {
        const string azureFunctionUrl = "https://cdscryptography.azurewebsites.net/api/HttpTrigger1";
        static readonly HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(azureFunctionUrl);

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
