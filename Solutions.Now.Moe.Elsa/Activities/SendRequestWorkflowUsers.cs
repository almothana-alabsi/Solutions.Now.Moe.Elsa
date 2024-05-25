using Amazon.AWSSupport.Model;
using Elsa.Attributes;
using Elsa;
using Solutions.Now.Moe.Elsa.Models;
using Elsa.Services;
using Elsa.ActivityResults;
using Elsa.Expressions;
using Elsa.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Solutions.Now.Moe.Elsa.Models;
using System;
using Solutions.Now.Moe.Elsa.Common;
using Positions = Solutions.Now.Moe.Elsa.Common.Positions;
using System.Runtime.Intrinsics.Arm;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

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
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                //HttpClientHandler handler = new HttpClientHandler
                //{
                //    ServerCertificateCustomValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; },
                //};

                using (var httpClient = new HttpClient())
                {
                    string URL = connectionString + "/api/WorkFlows/Request/" + WorkFlowSignal + "/" + RequestSerial.ToString() + "/" + userName;


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
