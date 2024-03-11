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

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
       Category = "Construction",
       DisplayName = "Communication Notification Global Member Approval",
       Description = "Communication Notification Global Member Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class ComitteeMemberNotfications : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public ComitteeMemberNotfications(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the workflow type.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? workflowType { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the type.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? type { get; set; }



        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<string> committeemember = new List<string>();
            HashSet<string> userNameDB = new HashSet<string>();
            try
            {
                if (workflowType == 3756)
                {
                    ChangeOrder changeOrders = await _moeDBContext.ChangeOrder.FirstOrDefaultAsync(x => x.Serial == RequestSerial);
                    //                    Committee committees = await _moeDBContext.Committee.AsAsyncEnumerable().Where(c => c.TenderSerial == changeOrders.tenderSerial && c.committeeType == workflowType).ToListAsync();
                    Committee committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.TenderSerial == changeOrders.tenderSerial && i.committeeType==workflowType);

                    var committeeMembers = await _moeDBContext.CommitteeMember.AsAsyncEnumerable().Where(y => y.committeeSerial == committee.Serial).ToListAsync();
                    for (int i = 0; i < committeeMembers.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(committeeMembers[i].userName.ToString()))
                        {
                            TblUsers users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.username == committeeMembers[i].userName);
                            committeemember.Add(committeeMembers[i].userName);

                        }

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
