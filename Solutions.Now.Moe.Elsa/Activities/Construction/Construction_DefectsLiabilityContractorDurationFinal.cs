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
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.CMIS2.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "Defects Liability Contractor Duration final approvals",
          Description = "Add Users of Defects Liability Contractor Duration1 final approvals",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_DefectsLiabilityContractorDurationFinal : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
  
        private readonly IClock _clock;

        public Construction_DefectsLiabilityContractorDurationFinal(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext, IClock clock)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;
            _clock = clock;
        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        public int? finalDuration { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            try
            {

                var finalDuration = await _ConstructionDBContext.detailsOfFinalReceiptCommittee.FirstOrDefaultAsync(c => c.finalReceiptSerial == RequestSerial);
                context.Output = finalDuration.durationForMissingDataContractor;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Done();
        }

    }
}
