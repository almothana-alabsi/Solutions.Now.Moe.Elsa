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
using Solutions.Now.Moe.Elsa.Models.Construction;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
       Category = "Construction",
       DisplayName = "Get Concern User Approval",
       Description = "Get Concern User Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class Construction_GetConcernUserMoeUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        private readonly IConfiguration _configuration;


        public Construction_GetConcernUserMoeUsers(IConfiguration configuration,ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
                _moeDBContext = moeDBContext;
                _configuration = configuration;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Table Name.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string? tableName { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Column Name.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string? columnName { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Column Name for Where Clause.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string? columnNameForWhereClause { get; set; }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnectionMoe");
            var userName = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT "+ columnName + " FROM Moe.Construction."+ tableName + " where "+ columnNameForWhereClause+" = "+RequestSerial;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            userName = reader.GetString(0);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
           context.Output = userName;
            return Done();
        }
    }
}
