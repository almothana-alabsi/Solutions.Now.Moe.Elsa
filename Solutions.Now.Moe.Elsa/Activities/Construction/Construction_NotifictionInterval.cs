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
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Solutions.Now.Moe.Elsa.Models.Construction;
using Solutions.Now.Moe.Elsa.Integrations;
using System.Net.Http;
using System.Net;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
      Category = "Notifiction",
      DisplayName = "Construction_Notifiction Interval",
      Description = "Construction_Notifiction in ApprovalHistory Table",
      Outcomes = new[] { OutcomeNames.Done }
  )]
    public class Construction_NotifictionInterval : Activity
    {

        private readonly ConstructionDBContext _moeDBContext;
        private readonly IConfiguration _configuration;
        private readonly SsoDBContext _databaseConnectionSSO;
        private Email _email;

        public Construction_NotifictionInterval(IConfiguration configuration, ConstructionDBContext moeDBContext, SsoDBContext databaseConnectionSSO, Email email)
        {
            _moeDBContext = moeDBContext;
            _configuration = configuration;
            _databaseConnectionSSO = databaseConnectionSSO;
            _email = email;
        }


        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Type.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestType {  get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Steps.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public object steps { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Names.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public object userNameDB { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Forms.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public object forms { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to Status.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int Status { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to From.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int from { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to To.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int to { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the refserial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? refSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnectionMoe");

            if (userNameDB == null) { return Done(); };
            IList<int> _stepsList = new List<int>();
            IList<string> _userNameList = new List<string>();
            IList<string> _formsList = new List<string>();
            string word = "", _steps = (string)steps, _userName = (string)userNameDB, _forms = (string)forms;
            for (int i = 0; i < _steps.Length; i++)
            {
                if (_steps[i].ToString().Equals(","))
                {
                    _stepsList.Add(Int32.Parse(word));
                    word = "";
                }
                else
                {
                    word += _steps[i];
                }
                if (i == _steps.Length - 1)
                {
                    _stepsList.Add(Int32.Parse(word));
                    word = "";
                }
            }
            for (int i = 0; i < _userName.Length; i++)
            {
                if (_userName[i].ToString().Equals(","))
                {
                    _userNameList.Add(word);
                    word = "";
                }
                else
                {
                    word += _userName[i];
                }
                if (i == _userName.Length - 1)
                {
                    _userNameList.Add(word);
                    word = "";
                }
            }
            for (int i = 0; i < _forms.Length; i++)
            {
                if (_forms[i].ToString().Equals(","))
                {
                    _formsList.Add(word);
                    word = "";
                }
                else
                {
                    word += _forms[i];
                }
                if (i == _forms.Length - 1)
                {
                    _formsList.Add(word);
                    word = "";
                }
            }

            for (int i = from; i < Math.Min(to + 1, _userNameList.Count); i++)
            {
                int mn = Math.Min(i, _stepsList.Count - 1);
                    var approvalHistory = new ApprovalHistory
                {
                    step = _stepsList[mn],
                    requestSerial = RequestSerial,
                    requestType = RequestType,
                    createdDate = DateTime.Now,
                    actionBy = _userNameList[i],
                    actionDate = DateTime.Now,
                    expireDate = DateTime.Today.AddDays(10),
                    Form = _formsList[0] + RequestSerial,
                    status = Status,
                    seen = null,
                    refSerial = refSerial
                };
                try
                {

                   // var @connectionString = "Server=207.180.223.162;Uid=Sa;Pwd=SolNowDev@#25;Database=Moe";
                    SqlConnection connection = new SqlConnection(connectionString);


                        string query = "INSERT INTO [Moe].[Construction].[ApprovalHistory] ([requestserial] ,[requestType] ,[createdDate],[actionBy],[actionDate],[expireDate],[status],[URL],[Form],[step]) ";
                        query = query + " values (" + approvalHistory.requestSerial + ", " + approvalHistory.requestType + ",  GETDATE(), '" + approvalHistory.actionBy + "', GETDATE(), GETDATE() , " + approvalHistory.status + ", '" + approvalHistory.URL + "', '" + approvalHistory.Form + "', " + approvalHistory.step + ");";
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

                                    string url = apiUrlSMS + user.phoneNumber.ToString() + "&createdBy=" + approvalHistory.actionBy.ToString() + "&requsetType=" + RequestType.ToString() + " &requestSerial=" + approvalHistory.requestSerial.ToString() + "&lang=ar&isFYI=1";


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
                                            string url = await _email.SendEmail(approvalHistory.actionBy, RequestType, approvalHistory.requestSerial, "ar", 1);

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
            }
            return Done();


        }
    }
}
