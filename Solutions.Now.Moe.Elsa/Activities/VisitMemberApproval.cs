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
       DisplayName = "Visit Member Approval",
       Description = "Visit Member Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class VisitMemberApproval : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public VisitMemberApproval(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {

            HashSet<string> userNameDB = new HashSet<string>();
            List<VisitMember> visitMember = _moeDBContext.visitMember.AsQueryable().Where(s => s.siteVisitSerial == RequestSerial).ToList<VisitMember>();
            try
            {
                for (int i = 0; i < visitMember.Count; i++)
                {
                    if (!String.IsNullOrEmpty(visitMember[i].name.ToString()))
                    {
                        TblUsers users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.username == visitMember[i].name);
                        userNameDB.Add(users.username);

                    }

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            DataForRequestProject infoX = new DataForRequestProject
            {
                name = userNameDB.ToList<string>()
            };
            context.Output = infoX;
            return Done();
        }
    }
}
