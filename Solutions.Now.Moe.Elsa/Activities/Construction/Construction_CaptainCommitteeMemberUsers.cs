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

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
       Category = "Construction",
       DisplayName = "Captain Committee Member Approval",
       Description = "Captain Committee Member Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class Construction_CaptainCommitteeMemberUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_CaptainCommitteeMemberUsers(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the workflow type.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? workflowType { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            try
            {
                var captainCommittee = _ConstructionDBContext.CommitteeMember.FirstOrDefault(x=>x.masterSerial == RequestSerial && x.type == workflowType && x.captain == 1);
                context.Output = captainCommittee.userName;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Done();
        }
    }
}
