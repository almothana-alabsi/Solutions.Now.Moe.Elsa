using Elsa.Attributes;
using Elsa;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Common;
using Elsa.Services;
using Elsa.ActivityResults;
using Elsa.Expressions;
using Elsa.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Esprima;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Flags",
          DisplayName = "projects Tender stages flags approval",
          Description = "Add approval in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class projectsTenderFlagsApproval : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public projectsTenderFlagsApproval(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Stage", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int Stage { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            TblUsers users;
            int flagcommitteeMemberSurveyEng = 0;

            try
            {
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.DeliveryOfStage && s.type == WorkFlowType.WorkflowType).OrderBy(s => s.step).ToList<WorkFlowRules>();
                var projects = await _moeDBContext.ProjectsTender.FirstOrDefaultAsync(i => i.Serial == RequestSerial);
                if (projects.stage == 1) { flagcommitteeMemberSurveyEng = 1; }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            projectsTenderFlagsDTO infoX = new projectsTenderFlagsDTO
            {
                committeeMemberSurveyEng = flagcommitteeMemberSurveyEng

            };
            context.Output = infoX;
            return Done();
        }
    }
}

