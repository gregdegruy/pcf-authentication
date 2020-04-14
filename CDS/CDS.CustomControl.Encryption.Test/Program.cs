using CDS.CustomControl.Encryption.Test.Authentication;
using System.Configuration;
using System;

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

            powerApp.GetSolutions();
        }
    }
}
