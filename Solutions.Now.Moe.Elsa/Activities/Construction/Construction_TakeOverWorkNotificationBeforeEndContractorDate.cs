using Amazon.ElasticMapReduce.Model;
using Elsa;
using Elsa.Activities.Temporal;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Data.SqlClient;
using Solutions.Now.Moe.Elsa.Models;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Solutions.Now.Moe.Elsa.Models.Construction;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
   Category = "Approval",
   DisplayName = "Take Over Work Notification Before End Contractor Date Workflow",
   Description = "Take Over Work Notification Before End Contractor Date Workflow Table",
   Outcomes = new[] { OutcomeNames.Done }
)]
    public class Construction_TakeOverWorkNotificationBeforeEndContractorDate : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;

        public Construction_TakeOverWorkNotificationBeforeEndContractorDate(ConstructionDBContext ConstructionDBContext)
        {
            _ConstructionDBContext = ConstructionDBContext;
        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            try
            {
                Construction_detailsOfTakeOverCommittee initialReceipt = await _ConstructionDBContext.detailsOfTakeOverCommittee.OrderBy(x=>x.serial).LastOrDefaultAsync(r => r.takeOverSerial == RequestSerial);
                DateTime? dateTimeNow = DateTime.Now;
                var endDate = initialReceipt.contractorResponsibilityEndDate;
                var dateInitialNow = ((endDate != null ? endDate : DateTime.Now) - dateTimeNow).Value.Days;
                if (dateInitialNow >= 90)
                {
                    int durationNotification = dateInitialNow - 90;
                    context.Output = durationNotification;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message.ToString());
            }
            return Done();
        }
    }
}
