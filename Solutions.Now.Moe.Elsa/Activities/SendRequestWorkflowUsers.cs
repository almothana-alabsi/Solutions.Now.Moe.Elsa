using Amazon.AWSSupport.Model;
using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Solutions.Now.Moe.Elsa.Common;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Positions = Solutions.Now.Moe.Elsa.Common.Positions;

namespace Solutions.Now.CMIS2.Elsa.Activities
{
    [Activity(
   Category = "Approval",
   DisplayName = "Send Request",
   Description = "Send Request to Workflow Table",
   Outcomes = new[] { OutcomeNames.Done }
)]
    public class SendRequestWorkflowUsers : Activity
    {
        private readonly MoeDBContext _moeDBContext;
        private readonly IConfiguration _configuration;
        public SendRequestWorkflowUsers(IConfiguration configuration, MoeDBContext moeDBContext)
        {
            _moeDBContext = moeDBContext;
            _configuration = configuration;
        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the WorkFlow Signal.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string WorkFlowSignal { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the User Name.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string userName { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            string connectionString = _configuration.GetValue<string>("Elsa:Server:BaseUrl");

            try
            {
                //system.net.servicepointmanager.securityprotocol = securityprotocoltype.tls12 | securityprotocoltype.tls11 | securityprotocoltype.tls;
                //httpclienthandler handler = new httpclienthandler
                //{
                //    servercertificatecustomvalidationcallback = (senderx, certificate, chain, sslpolicyerrors) => { return true; },
                //};


                int flagSSL = int.Parse(_configuration["Elsa:Server:flagSSL"]);

                HttpClientHandler handler = new HttpClientHandler();

                if (flagSSL == 1)
                {
                    // Enable TLS protocols
                    System.Net.ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    // Ignore SSL certificate errors (not recommended for production)
                    handler.ServerCertificateCustomValidationCallback =
                        (senderX, certificate, chain, sslPolicyErrors) => true;
                }

                using (var httpClient = new HttpClient(handler)) // ✅ pass handler here
                {
                    string URL = connectionString.EndsWith("/")
                        ? connectionString + "api/WorkFlows/Request/" + WorkFlowSignal + "/" + RequestSerial + "/" + userName
                        : connectionString + "/api/WorkFlows/Request/" + WorkFlowSignal + "/" + RequestSerial + "/" + userName;

                    HttpResponseMessage response = await httpClient.GetAsync(URL);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Successfully send");
                    }
                    else
                    {
                        Console.WriteLine("Failure send");
                    }
                }

                string urlEmpty = _configuration.GetValue<string>("Server:URL");
                context.Output = urlEmpty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message.ToString());
            }
            return Done();
        }
    }
}
