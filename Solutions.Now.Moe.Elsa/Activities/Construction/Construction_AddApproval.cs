using Elsa;
using Elsa.Services;
using Elsa.Attributes;
using Solutions.Now.Moe.Elsa.Models;
using Microsoft.Data.SqlClient;
using System;
using Elsa.Expressions;
using Elsa.ActivityResults;
using System.Threading.Tasks;
using Elsa.Services.Models;
using Microsoft.Extensions.Configuration;
using Solutions.Now.Moe.Elsa.Models.Construction;
using System.Net.Http;
using System.Net;
using Amazon.CloudTrail.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Solutions.Now.Moe.Elsa.Integrations;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
       Category = "Construction",
       DisplayName = "Add approval",
       Description = "Add approval in Construction ApprovalHistory Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class Construction_AddApproval : Activity
    {
        private readonly ConstructionDBContext _moeDBContext;
        private readonly IConfiguration _configuration;
        private readonly SsoDBContext _databaseConnectionSSO;
        private Email _email;
        public Construction_AddApproval(IConfiguration configuration, ConstructionDBContext MoeDBContext, SsoDBContext databaseConnectionSSO,Email email)
        {
            _moeDBContext = MoeDBContext;
            _configuration = configuration;
            _databaseConnectionSSO = databaseConnectionSSO;
            _email = email;
        }


        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Type.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestType { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Step.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int Step { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Name.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string ActionBy { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to URL.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string URL { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Form.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string Form { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the refserial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? refSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Major.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? Major { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Status.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? Status { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the CreatedBy.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string? createdBy { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnectionMoe");

            try
                {
                if (Status == null) { Status = 387; };
                ApprovalHistory approvalHistory = new ApprovalHistory
                {
                    requestSerial = RequestSerial,
                    requestType = RequestType,
                    createdDate = DateTime.Now,
                    actionBy = ActionBy,
                    expireDate = DateTime.Today.AddDays(10),
                    step = Step,
                    URL = URL,  
                    Form = Form + RequestSerial.ToString(),
                    status = Status,
                    ActionDetails = (Major != null ? Major : -1),
                    refSerial = refSerial,
                    createdBy = createdBy
                };
                //await _cmis2DbContext.ApprovalHistory.AddAsync(approvalHistory);
                // await _cmis2DbContext.SaveChangesAsync();
               // var @connectionString = "Server=207.180.223.162;Uid=Sa;Pwd=SolNowDev@#25;Database=Moe";
                SqlConnection connection = new SqlConnection(connectionString);

   
                     string query = "INSERT INTO [Moe].[Construction].[ApprovalHistory] ([requestserial] ,[requestType] ,[createdDate],[actionBy],[expireDate],[status],[URL],[Form],[step],[ActionDetails],createdBy) ";
                    query = query + " values (" + approvalHistory.requestSerial + ", " + approvalHistory.requestType + ",  GETDATE(), '" + approvalHistory.actionBy + "', GETDATE()+10 , " + approvalHistory.status + ", '" + approvalHistory.URL + "', '" + approvalHistory.Form + "', " + approvalHistory.step + "," + approvalHistory.ActionDetails + ",'" + approvalHistory.createdBy + "');";
                    SqlCommand command = new SqlCommand(query, connection);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        Console.WriteLine("Records Inserted Successfully");
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine("Error Generated. Details: " + e.ToString());
                    }
                    finally
                    {
                        connection.Close();
                   }
                var user = await _databaseConnectionSSO.TblUsers.OrderBy(x => x.serial).FirstOrDefaultAsync(y => y.username.ToLower().Equals(approvalHistory.actionBy.ToLower()));
                if (Int32.Parse(_configuration["SMS:flag"]) == 1)
                {
                    if (user != null)
                    {
                        if (user.phoneNumber != null)
                        {
                            if (user.phoneNumber.Length == 12 && user.phoneNumber.StartsWith("962"))
                            {
                                string apiUrlSMS = _configuration["SMS:URL"];

                                string url = apiUrlSMS + user.phoneNumber.ToString() + "&createdBy=" + approvalHistory.actionBy.ToString() + "&requsetType=" + RequestType.ToString() + " &requestSerial=" + approvalHistory.requestSerial.ToString() + "&lang=ar&isFYI=0";


                                //https://localhost:7149/api/SMS?phoneNumber=962776535312&createdBy=osama&requsetType=3766&requestSerial=11633&lang=ar
                                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                                HttpClientHandler handler = new HttpClientHandler
                                {
                                    ServerCertificateCustomValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; },
                                };

                                using (var httpClient = new HttpClient(handler))
                                {
                                    HttpResponseMessage response = await httpClient.GetAsync(url);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine("Successfully send");

                                    }
                                    else
                                    {
                                        Console.WriteLine("failer send");

                                    }
                                }
                            }
                        }
                        if (Int32.Parse(_configuration["EmailApi:flag"]) == 1)
                        {
                            if (user.email != null)
                            {
                                if (_email.IsValidEmail(user.email))
                                {
                                    HttpClientHandler handler = new HttpClientHandler();
                                    if (!string.IsNullOrEmpty(_configuration["EmailApi:Proxy"]))
                                    {

                                        handler.Proxy = new WebProxy(_configuration["EmailApi:Proxy"]);
                                    }

                                    using (var httpClient = new HttpClient(handler))
                                    {
                                        string url = await _email.SendEmail(approvalHistory.actionBy, RequestType, approvalHistory.requestSerial, "ar", 0, user.email);

                                        HttpResponseMessage response = await httpClient.GetAsync(url);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            Console.WriteLine("Successfully send");

                                        }
                                        else
                                        {
                                            Console.WriteLine("failer send");

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message.ToString());
            }
            return Done();
        }
    }
}
