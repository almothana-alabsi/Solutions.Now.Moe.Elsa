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
using Microsoft.EntityFrameworkCore;
using Solutions.Now.Moe.Elsa.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace Solutions.Now.DesignReviewAndPlanning.Elsa.Activities
{
    [Activity(
        Category = "Get Tender Serial Approval",
        Description = "Get  Tender Serial Approval",
        Outcomes = new[] { OutcomeNames.Done }
        )]

    public class Construction_GetTenderSerial : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        private readonly IConfiguration _configuration;

        public Construction_GetTenderSerial(IConfiguration configuration, ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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
            string connectionString = _configuration.GetConnectionString("DefaultConnectionDesignReview");
            int exchangeParty = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT " + columnName + " FROM MOE.Construction." + tableName + " where " + columnNameForWhereClause + " = " + RequestSerial;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            exchangeParty = reader.GetInt32(0);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            var exchangePartyData = new
            {
                number = exchangeParty

            };
            context.Output = exchangePartyData;
            return Done();
        }

    }
}
