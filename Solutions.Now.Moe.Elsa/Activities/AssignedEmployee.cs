using Amazon.AWSSupport.Model;
using Elsa.Attributes;
using Elsa;
using Solutions.Now.Moe.Elsa.Models;
using Elsa.Services;
using Elsa.Expressions;
using Elsa.ActivityResults;
using Elsa.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Solutions.Now.Moe.Elsa.Common;
using System;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "AssignedEmployee",
          Description = "Committee Member Approval in WorkflowRules Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class AssignedEmployee : Activity
    {

        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public AssignedEmployee(SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            TblUsers tblUsers = new TblUsers();
            try { 
                int requestSerial = RequestSerial;
                ReferedTender referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(u => u.Serial == requestSerial);
                tblUsers = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(s => s.username.Equals(referedTender.maintenanceEngineer));
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            context.Output = tblUsers.username;
            return Done();
        }
    }
}
