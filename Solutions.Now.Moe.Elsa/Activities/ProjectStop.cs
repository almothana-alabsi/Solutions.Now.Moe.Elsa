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
using Solutions.Now.Moe.Elsa.Common;
namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
         Category = "Approval",
         DisplayName = "projectStopped",
         Description = "projectStopped",
         Outcomes = new[] { OutcomeNames.Done }
     )]
    public class projectStop : Activity
    {

        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public projectStop(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            const int warrantyMaintenanceWorkSerial = 4743;
            const int typeAction = 393;
            List<string> EmailDB = new List<string>();
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();

            List<string> Screen = new List<string>();
            try
            {

                //List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => (s.workflow == warrantyMaintenanceWorkSerial)).OrderBy(s => s.step).ToList<WorkFlowRules>();
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.projectStopped).OrderBy(s => s.step).ToList<WorkFlowRules>();

                TblUsers users;
                for (int i = 0; i < workFlowRules.Count; i++)
                {
                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }
                projectStoppedTable projectStopped = await _moeDBContext.ProjectStoppedTable.FirstOrDefaultAsync(i => i.serial == RequestSerial);


        
                    // projectStoppedTable projectStopped = await _moeDBContext.projectStoppedTable.FirstOrDefaultAsync(i => i.serial == RequestSerial);
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.sectionHead && u.Section == Hierarchy.section);
                    userNameDB[0] = users.username;
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.sectionHead && u.Section == Hierarchy.ExpenseSection);
                    userNameDB[1] = users.username;
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.DirectorateHead && u.Directorate == Hierarchy.Directorate);
                    userNameDB[2] = users.username;
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.DirectorateHead && u.Directorate == Hierarchy.AccountsDirectorate);
                    userNameDB[3] = users.username;
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.AdministrationHead && u.Administration == Hierarchy.AdminstratorFinancial);
                    userNameDB[4] = users.username;
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.AdministrationHead && u.Administration == Hierarchy.Administration);
                    userNameDB[5] = users.username;

                    if (projectStopped.serialTender != null)
                    {
                     
                        ReferedTender referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(i => i.Serial == projectStopped.serialTender);
                        users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Consultant == referedTender.Consultant);
                        userNameDB[6] = users.username;
                    }

                
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            DataForRequestProject infoX = new DataForRequestProject
            {
                requestSerial = RequestSerial,

                steps = steps,
                name = userNameDB,
                Screens = Screen,

            };
            context.Output = infoX;
            return Done();
        }
    }
}