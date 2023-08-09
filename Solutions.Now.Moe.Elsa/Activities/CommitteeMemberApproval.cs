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

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
       Category = "Approval",
       DisplayName = "Committee Member Approval",
       Description = "Committee Member Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class CommitteeMemberApproval : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public CommitteeMemberApproval(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<string> committeemember = new List<string>();
            HashSet<string> userNameDB = new HashSet<string>();
            List<CommitteeMember> committeeMembers = _moeDBContext.CommitteeMember.AsQueryable().Where(s => s.committeeSerial == RequestSerial).ToList<CommitteeMember>();
            try
            {
                for (int i = 0; i < committeeMembers.Count; i++)
                {
                    if (!String.IsNullOrEmpty(committeeMembers[i].userName.ToString()))
                    {
                        //TblUsers users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.username == committeeMember[i].userName);
                        committeemember.Add(committeeMembers[i].userName);

                    }

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            DataForRequestProject infoX = new DataForRequestProject
            {
                name = committeemember
            };
            context.Output = infoX;
            return Done();
        }
    }
}
