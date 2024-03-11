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
       Category = "DesignReview",
       DisplayName = "Captain Committee Approval",
       Description = "Captain Committee Member Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class DesignReviewCommiteMember : Activity
    {
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public DesignReviewCommiteMember( SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? RequestSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            try
            {
                var captainCommittee = _moeDBContext.CommitteeMember.FirstOrDefault(x => x.committeeSerial == RequestSerial && x.capten == 1);
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
