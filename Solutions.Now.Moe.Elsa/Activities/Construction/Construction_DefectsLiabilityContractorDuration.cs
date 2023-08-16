using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Expressions;
using System;
using NodaTime;
using Elsa.Builders;
using Elsa.Activities.Temporal;
using Solutions.Now.Moe.Elsa.Models.Construction;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Common;

namespace Solutions.Now.CMIS2.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "Defects Liability Contractor Duration approvals",
          Description = "Add Users of Defects Liability Contractor Duration1 approvals",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_DefectsLiabilityContractorDuration : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
  
        private readonly IClock _clock;

        public Construction_DefectsLiabilityContractorDuration(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext, IClock clock)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;
            _clock = clock;
        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        public int? durations { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {


            IList<int?> steps = new List<int?>();
            IList<string> userNameDB = new List<string>();
            IList<string> forms = new List<string>();

            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_partial_Receipt_Works).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();

            for (int i = 0; i < workFlowRules.Count; i++)
            {
                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                forms.Add(workFlowRules[i].screen);
            }
            try
            {
            

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            context.Output = durations;
            return Done();
        }

    }
}
