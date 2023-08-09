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
using System;
using Solutions.Now.Moe.Elsa.Common;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
      Category = "Account user",
      DisplayName = " AccountUser",
      Description = "Account in ApprovalHistory Table",
      Outcomes = new[] { OutcomeNames.Done }
  )]
    public class ApproveAcountantuser : Activity
    {
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public ApproveAcountantuser(SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        { 
            TblUsers users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.Accountant);   
            //Accountant
            context.Output = users.username;
            return Done();
        }
    }
}
