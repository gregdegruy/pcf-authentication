namespace CDS.CustomControl.Encryption.Test.Authentication
{
    public class PowerPlatformEnvironment
    {
        public string ServerAddress { get; }
        public string User { get; }
        public string Password { get; }

        public PowerPlatformEnvironment(string serverAddress, string user, string password, 
            string publisherUniqueName, string publisherPrefix)
        {
            ServerAddress = serverAddress;
            User = user;
            Password = password;
        }
    }
}
