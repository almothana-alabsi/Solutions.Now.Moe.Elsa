using Microsoft.Extensions.Configuration;
using Solutions.Now.Moe.Elsa.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Solutions.Now.Moe.Elsa.Models.Construction;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Solutions.Now.Moe.Elsa.Integrations
{

    public class Email
    {

        private readonly ConstructionDBContext _moeDBContext;
        private readonly SsoDBContext _SsoDBContext;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public Email(IConfiguration configuration, ConstructionDBContext MoeDBContext, SsoDBContext ssoDBContext)
        {
            _moeDBContext = MoeDBContext;
            _configuration = configuration;
            _SsoDBContext = ssoDBContext;
        }
        public async Task<string> SendEmail(string actionBy, int requsetType, int? requestSerial, string lang, int isFYI)
        {
            string URL = _configuration["EmailApi:URL"];
            string descEn = "";
            string descAr = "";
            string urlEmail = "";
            if (isFYI == 1)
            {
                descAr = _configuration["EmailApi:descArFYI"];
                descEn = _configuration["EmailApi:descEnFYI"];

            }
            else
            {
                descAr = _configuration["EmailApi:descAr"];
                descEn = _configuration["EmailApi:descEn"];
            }

            try
            {
                var langFilter = lang.ToLower();
                string descMSG = "";
                var desc = await _SsoDBContext.MasterData.OrderBy(y => y.serial).FirstOrDefaultAsync(x => x.serial == requsetType);
                if (requestSerial != null)
                {
                        if (langFilter == "en")
                        {
                            descMSG = descEn + " " + desc.descEN;
                        }
                        else
                        {
                            descMSG = descAr + " " + desc.descAR;

                        }
                    
                    var user = await _SsoDBContext.TblUsers.OrderBy(x => x.serial).FirstOrDefaultAsync(y => y.username.Equals(actionBy));
                    if (user != null)
                    {
                        if (user.email != null)
                        {
                            urlEmail = URL + user.email.ToString() + "&createdBy=" + actionBy.ToString() + "&lang=ar&descMSG=" + descMSG;

                        }
                    }

                }
                return urlEmail;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return urlEmail;
            }

        }
        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }

}
