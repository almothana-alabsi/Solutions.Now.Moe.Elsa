using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Expressions;
using System;
using NodaTime;
using Elsa.Builders;
using Elsa.Activities.Temporal;
using Solutions.Now.Moe.Elsa.Models.Construction;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{

    [Activity(
   Category = "Construction",
   DisplayName = "Construction_Send Request",
   Description = "Construction_Send Request to Workflow Table",
   Outcomes = new[] { OutcomeNames.Done }
)]
    public class ConstructionSendRequestWorkflowUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        private readonly IConfiguration _configuration;

        public ConstructionSendRequestWorkflowUsers(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext, IConfiguration configuration)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;
            _configuration = configuration;
        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the WorkFlow Signal.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string WorkFlowSignal { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the  commitee.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int commitee { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            string connectionString = _configuration.GetValue<string>("Elsa:Server:BaseUrl");

            try
            {


                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; },
                };

                using (var httpClient = new HttpClient(handler))
                {
                    string URL = String.Empty;

                    if (connectionString.EndsWith("/"))
                    {
                        URL = connectionString + "api/WorkFlows/Request/" + WorkFlowSignal + "/" + RequestSerial.ToString();
                    }
                    else
                    {
                        URL = connectionString + "/api/WorkFlows/Request/" + WorkFlowSignal + "/" + RequestSerial.ToString();
                    }

                    HttpResponseMessage response = await httpClient.GetAsync(URL);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Successfully send");

                    }
                    else
                    {
                        Console.WriteLine("failer send");

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