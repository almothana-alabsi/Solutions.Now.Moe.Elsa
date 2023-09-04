using Amazon.AWSSupport.Model;
using Amazon.CloudSearchDomain.Model;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services.Models;
using Elsa;
using NodaTime;
using Solutions.Now.Moe.Elsa.Models.Construction;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using Elsa.Services;
using System.Linq;
using Elsa.Builders;
using Elsa.Activities.Temporal;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Common;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
          Category = "Approval",
          DisplayName = "Defects Liability Contractor Duration1 approvals",
          Description = "Add Users of Defects Liability Contractor Duration1 approvals",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class DefectsLiabilityContractorDuration1 : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly IClock _clock;

        public DefectsLiabilityContractorDuration1(ConstructionDBContext cmis2DbContext, IClock clock)
        {
            _ConstructionDBContext = cmis2DbContext;
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

            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_InitialReceipt).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();

            for (int i = 0; i < workFlowRules.Count; i++)
            {
                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                forms.Add(workFlowRules[i].screen);
            }
            try
            {
                    
                Construction_detailsOfTakeOverCommittee warrantyMaintenanceWork = await _ConstructionDBContext.detailsOfTakeOverCommittee.OrderBy(x=>x.serial).LastOrDefaultAsync(i => i.takeOverSerial == RequestSerial);
                durations = warrantyMaintenanceWork.durationForMissingDataContractor;

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