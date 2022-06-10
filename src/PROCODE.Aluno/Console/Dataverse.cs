using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public class Dataverse
    {
        public Dataverse()
        {
            string appconfigPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(Path.GetFullPath($"{appconfigPath}\\{typeof(Dataverse).Namespace}\\app-sensitive.config")) };
            var config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;

            string url = settings[this.url].Value.ToString();
            string user = settings[this.user].Value.ToString();
            string password = settings[this.password].Value.ToString();
            string clientId = settings[this.clientId].Value.ToString();
            string clientSecret = settings[this.clientSecret].Value.ToString();

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
                this.Service = ClientSecret();
            else if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                this.Service = UserPassword();

            if (this.Service != null && this.Service.IsReady)
            {
            }
            else
                throw new System.Exception(this.Service.LastCrmException.Message);

            CrmServiceClient UserPassword()
            {
                string conn = $@"Url = {url};
                            AuthType = Office365;
                            UserName = {user};
                            Password = {password};
                            RequireNewInstance = True";

                return new CrmServiceClient(conn);
            }

            CrmServiceClient ClientSecret()
            {
                string conn = $@"Url={url};
                            AuthType=ClientSecret;
                            ClientId={clientId};
                            ClientSecret={clientSecret}";

                return new CrmServiceClient(conn);
            }
        }

        //Constantes
        public string url = "d365_url";
        public string user = "d365_user";
        public string password = "d365_password";
        public string clientId = "d365_clientid";
        public string clientSecret = "d365_clientsecret";

        public enum eAuthType
        {
            UserPassword = 0,
            ClientSecret = 1
        }

        public CrmServiceClient Service { get; private set; }
    }
}
