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
using Solutions.Now.Moe.Elsa.Models.Construction;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
          Category = "Construction",
          DisplayName = "Construction Insurance Policy users approval",
          Description = "Construction Insurance Policy users in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_InsurancePolicyUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_InsurancePolicyUsers(ConstructionDBContext ConstructionDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = ConstructionDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Sender.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string RequestSender { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_InsurancePolicyUsers).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

            for (int i = 0; i < workFlowRules.Count; i++)
            {

                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screen.Add(workFlowRules[i].screen);
            }

            try
            {
                var insurancePolicy = await _ConstructionDBContext.InsurancePolicy.FirstOrDefaultAsync(x => x.serial == RequestSerial);
                var tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(x => x.tenderSerial == insurancePolicy.tenderSerial);
                //المقاول
                userNameDB[0] = RequestSender;
                //رئيس اللجنة
                var committeeCaptain = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == insurancePolicy.tenderSerial && x.type == WorkFlowsName.Construction_SupervisionCommittee && x.captain == 1);
                userNameDB[1] = committeeCaptain.userName;
                //رئيس قسم الابنية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.sectionBuilding && u.position == Positions.sectionHead && u.organization == Organization.MOE);
                if (users != null)
                {
                    userNameDB[2] = users.username;
                }
                //مدير ادارة الشؤون المالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead && (u.organization == 2 || u.organization == 3));
                if (users != null)
                {
                    userNameDB[3] = userNameDB[5] = users.username;
                }
                else
                {
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.AdministrationHead && u.organization == 2);
                    if (users != null)
                    {
                        userNameDB[3] = userNameDB[5] = users.username;
                    }
                }
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead && u.organization == Organization.MOE);
                if (users != null)
                {
                    userNameDB[4] = users.username;
                }
                //مدير مديرية الحسابات
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.AccountsDirectorate && u.position == Positions.DirectorateHead && u.organization == 2);
                userNameDB[6] = users.username;
                //رئيس قسم النفقات والمخصصات
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.ExpenseSection && u.position == Positions.sectionHead && u.organization == 2);
                userNameDB[7] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.Accountant && u.organization == Organization.MOE);
                //Accountant
                userNameDB[8] = users.username;
                // رئيس قسم المالية في مديرية التربية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Directorate == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.Section == Hierarchy.sectionFinanial && u.position == Positions.sectionHead);
                if (users != null)
                {
                    userNameDB[9] = users.username;
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
                RequestSender = RequestSender

            };
            context.Output = infoX;
            return Done();
        }
    }
}

