using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace CDS.CustomControl.Encryption.Test
{
    public class UnitTest
    {
        [Fact]
        public void CreateCDSUserHash()
        {
            string cdsUserToken = "";
            string systemUserId = "00000000-0000-0000-0000-000000000000";
            string internalemailaddress = "supermario@mushroom.kingdom";
            string azureActiveDirectoryObjectId = "00000000-0000-0000-0000-000000000001";
            string input = "";
            input += systemUserId;
            input += internalemailaddress;
            input += azureActiveDirectoryObjectId;
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // "x2" for lowercase
                    sb.Append(b.ToString("X2"));
                }
                cdsUserToken = sb.ToString();
            }
            Assert.Equal("E1FCDE486F3A17D64C33889AB9EDD3681E4FA40C", cdsUserToken);
        }
    }
}
