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
          DisplayName = "Committee Communication FYI approvals",
          Description = "Committee Communication FYI approvals",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_TakOverComitteeFYI : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;

        public Construction_TakOverComitteeFYI(ConstructionDBContext cmis2DbContext)
        {
            _ConstructionDBContext = cmis2DbContext; 
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

                Construction_detailsOfTakeOverCommittee warrantyMaintenanceWork = await _ConstructionDBContext.detailsOfTakeOverCommittee
                .OrderBy(x => x.serial)
                .LastOrDefaultAsync(i => i.takeOverSerial == RequestSerial);
                DateTime dateTimeNow = DateTime.Now;
                var endDate = warrantyMaintenanceWork.contractorResponsibilityEndDate;
                var dateInitialNow = ((endDate != null ? endDate : DateTime.Now) - dateTimeNow).Value.Days;
              

                   if(dateInitialNow <= 60)
                {
                    dateInitialNow = 0;
                }
                else
                {
                    dateInitialNow = dateInitialNow - 60;
                }
                context.Output = dateInitialNow; 
                //}
                  
                    
            }
            catch (Exception ex)
            {
                ex.Message.ToString();  
            }
            
            return Done();
        }   

    }
    
    
} 

