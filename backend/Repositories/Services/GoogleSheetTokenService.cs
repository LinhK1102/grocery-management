using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Services
{
    public class GoogleSheetTokenService
    {
        private readonly string _credentialsPath;

        public GoogleSheetTokenService(string credentialsPath)
        {
            _credentialsPath = credentialsPath;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var credential = GoogleCredential.FromFile(_credentialsPath)
                .CreateScoped("https://www.googleapis.com/auth/spreadsheets");

            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
    }
}
