using CDS.CustomControl.Encryption.Test.Authentication;
using System.Configuration;
using Xunit;

namespace CDS.CustomControl.Encryption.Test
{
    public class IntegrationTest
    {
        [Fact]
        public void CDSAuthentication()
        {
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
