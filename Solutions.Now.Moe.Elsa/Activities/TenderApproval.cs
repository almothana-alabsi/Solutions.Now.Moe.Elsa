using Amazon.AWSSupport.Model;
using Elsa.Attributes;
using Elsa;
using Solutions.Now.Moe.Elsa.Models;
using Elsa.Services;
using Elsa.Expressions;
using Elsa.ActivityResults;
using Elsa.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Solutions.Now.Moe.Elsa.Common;
using System;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
       Category = "Approval",
       DisplayName = "TenderApproval",
       Description = "Committee Member Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
    )]
    public class TenderApproval : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public TenderApproval(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            try
            {
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.PrepareTender && s.type == WorkFlowType.WorkflowType)
                                                    .OrderBy(s => s.step).ToList<WorkFlowRules>();
                TblUsers users;
                for (int i = 0; i < workFlowRules.Count; i++)
                {
                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }
                ReferedTender referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(i => i.Serial == RequestSerial);

                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.section && u.position == Positions.sectionHead);
                userNameDB[0] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.username.Equals(referedTender.assignedEngineer));
                userNameDB[1]  = (users == null) ? "NoEngineer" : users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead);
                userNameDB[2] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.maintenanceSection && u.position == Positions.sectionHead);
                userNameDB[3] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.maintenanceDepartment && u.position == Positions.DirectorateHead);
                userNameDB[5] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead);
                userNameDB[6] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SG);
                userNameDB[7] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.Minister);
                userNameDB[8] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.AdministrationHead);
                userNameDB[9] = users.username;
                //users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.AccountsDirectorate && u.position == Positions.DirectorateHead);
                userNameDB[10] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.ExpenseSection && u.position == Positions.sectionHead);
                userNameDB[11] = users.username;
              
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
