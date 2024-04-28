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

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
       Category = "Approval",
       DisplayName = "Add approval",
       Description = "Add approval in ApprovalHistory Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class AddApproval : Activity
    {
            private readonly MoeDBContext _moeDBContext;
            private readonly IConfiguration _configuration;
        public AddApproval(IConfiguration configuration, MoeDBContext MoeDBContext)
        {
            _moeDBContext = MoeDBContext;
            _configuration = configuration;
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
        [ActivityInput(Hint = "Enter an expression that evaluates to the Stage.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? Stage { get; set; }
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
                    ActionDetails = (Major != null? Major:-1),
                    refSerial = refSerial,
                    createdBy = createdBy,
                    Stage = Stage
                };
                //await _cmis2DbContext.ApprovalHistory.AddAsync(approvalHistory);
                // await _cmis2DbContext.SaveChangesAsync();
               // var @connectionString = "Server=185.193.17.20;Uid=Sa;Pwd=SolNowStg24@;Database=Moe";
                SqlConnection connection = new SqlConnection(connectionString);

                if (refSerial != null)
                {
                    string query = "INSERT INTO [Moe].[DesignReview].[ApprovalHistory] ([requestserial] ,[requestType] ,[createdDate],[actionBy],[expireDate],[status],[URL],[Form],[step],[refserial],[ActionDetails],createdBy,stage) ";
                    query = query + " values (" + approvalHistory.requestSerial + ", " + approvalHistory.requestType + ",  GETDATE(), '" + approvalHistory.actionBy + "', GETDATE()+10 , " + approvalHistory.status + ", '" + approvalHistory.URL + "', '" + approvalHistory.Form + "', " + approvalHistory.step + "," + approvalHistory.refSerial + "," + approvalHistory.ActionDetails + ",'" + approvalHistory.createdBy + "',IIF('' = '" + approvalHistory.Stage + "',null,'" + approvalHistory.Stage + "'));";
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
                else
                {
                    string query = "INSERT INTO [Moe].[DesignReview].[ApprovalHistory] ([requestserial] ,[requestType] ,[createdDate],[actionBy],[expireDate],[status],[URL],[Form],[step],[ActionDetails],createdBy,stage) ";
                    query = query + " values (" + approvalHistory.requestSerial + ", " + approvalHistory.requestType + ",  GETDATE(), '" + approvalHistory.actionBy + "', GETDATE()+10 , " + approvalHistory.status + ", '" + approvalHistory.URL + "', '" + approvalHistory.Form + "', " + approvalHistory.step + "," + approvalHistory.ActionDetails + ",'" + approvalHistory.createdBy + "',IIF('' = '" + approvalHistory.Stage + "',null,'" + approvalHistory.Stage + "'));";
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
                
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message.ToString());
            }
            return Done();
        }
    }
}
