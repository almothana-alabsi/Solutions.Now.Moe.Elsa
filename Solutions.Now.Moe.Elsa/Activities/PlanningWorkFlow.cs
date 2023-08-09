using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using MassTransit.RabbitMqTransport;
using NetTopologySuite.GeometriesGraph;
using Solutions.Now.Moe.Elsa.Common;
using Solutions.Now.Moe.Elsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Positions = Solutions.Now.Moe.Elsa.Common.Positions;

namespace Solutions.Now.Moe.Elsa.Activities
{

    [Activity(
         Category = "Approval",
         DisplayName = "Planning WorkFlow",
         Description = "Add Planning WorkFlow",
         Outcomes = new[] { OutcomeNames.Done }
     )]
    public class PlanningWorkFlow : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public PlanningWorkFlow(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            IList<int?> steps = new List<int?>();
            IList<string> userNameDB = new List<string>();
            IList<string> forms = new List<string>();         
            List<WorkFlowRules> workFlowRules =_moeDBContext.WorkFlowRules.AsQueryable().Where(s => (s.workflow == WorkFlowsName.PlanningProjects && s.type == WorkFlowType.WorkflowType)).OrderBy(s => s.step).ToList<WorkFlowRules>();

            
            for (int i = 0; i < workFlowRules.Count; i++)
            {
                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                forms.Add(workFlowRules[i].screen);

            }

            try
            {
            
              InfoProject infoProject = await _DesignReviewDBContext.InfoProject.FirstOrDefaultAsync(s => s.Serial == RequestSerial);
              TblUsers TblUsers = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.sectionHead && u.Section == Hierarchy.section);
              userNameDB[0] = TblUsers.username;
              TblUsers = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.DirectorateHead && u.Directorate == Hierarchy.Directorate);
              userNameDB[1] = TblUsers.username;
              TblUsers = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.AdministrationHead && u.Administration == Hierarchy.Administration);
                userNameDB[2] = TblUsers.username;
              TblUsers = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SG);
              userNameDB[3] = TblUsers.username;
                
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }  

            OutputActivityData infoX = new OutputActivityData
            {
                requestSerial = RequestSerial,
                steps = steps,
                names = userNameDB,
                Screens = forms,

            };
            context.Output = infoX;
            return Done();




        }

        }
}
