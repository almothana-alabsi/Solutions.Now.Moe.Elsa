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
        public IConfiguration Configuration { get; }

        public Construction_NotifictionInterval(ConstructionDBContext moeDBContext)
        {
            _moeDBContext = moeDBContext;
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
        { if (userNameDB == null) { return Done(); };
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

                    var @connectionString = "Server=207.180.223.162;Uid=Sa;Pwd=SolNowDev23;Database=Moe";
                    SqlConnection connection = new SqlConnection(@connectionString);


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
