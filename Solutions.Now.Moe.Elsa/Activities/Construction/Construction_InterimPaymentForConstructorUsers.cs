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
          DisplayName = "Construction Interim Payment For Constructor approval",
          Description = "Construction Interim Payment For Constructor in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_InterimPaymentForConstructorUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_InterimPaymentForConstructorUsers(ConstructionDBContext ConstructionDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_InterimPayment).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

            for (int i = 0; i < workFlowRules.Count; i++)
            {

                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screen.Add(workFlowRules[i].screen);
            }

            try
            {
                var invoicesPayment = await _ConstructionDBContext.InvoicesPayment.FirstOrDefaultAsync(x => x.Serial == RequestSerial);
                var tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(x => x.tenderSerial == 8);
                // المقاول
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.contractor == tender.tenderContracter1 && u.position == Positions.Contractor);
                userNameDB[0] = users.username;
                //رئيس قسم الابنية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.sectionBuilding && u.position == Positions.sectionHead);
                if (users != null)
                {
                    userNameDB[1] = users.username;
                }
                //رئيس اللجنة
                var committeeCaptain = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.projectSerial == RequestSerial && x.type == WorkFlowsName.Construction_SupervisionCommittee && x.captain == 1);
                userNameDB[4] = committeeCaptain.userName;
                // رئيس قسم المالية في مديرية التربية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.Section == Hierarchy.sectionFinanial && u.position == Positions.sectionHead);
                if (users != null)
                {
                    userNameDB[2] = users.username;
                }
                // رئيس قسم الرقابة الداخلية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Directorate == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.Section == Hierarchy.sectionDepartmentInternalControl && u.position == Positions.sectionHead);
                if (users != null)
                {
                    userNameDB[3] = users.username;
                }

                //مدير الشؤون الادارية والمالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Directorate == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.position == Positions.DirectorateHead);
                if (users != null)
                {
                    userNameDB[5] = users.username;
                }
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead);
                if (users != null)
                {
                    userNameDB[6] = users.username;
                }
                //رئيس قسم متابعة تنفيذ المشاريع المحلية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.Section == Hierarchy.sectionOfFollowUpToImplementationOfLocalProjectsSection && u.position == Positions.sectionHead && u.organization == 2);
                userNameDB[7] = users.username;
                //مهندس اتصال
                var CommunicationEng = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.projectSerial == RequestSerial && x.type == WorkFlowsName.Construction_CommunicationEng && x.captain == 1);
                userNameDB[8] = CommunicationEng.userName;
                //مدير مديرية الشؤون الهندسية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead && u.organization == 2);
                userNameDB[9] = users.username;
                //مدير ادارة الابنية والمشاريع الدولية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead && u.organization == 2);
                userNameDB[10] = users.username;
                //مدير ادارة الشؤون المالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.AdministrationHead && u.organization == 2);
                userNameDB[11] = users.username;
                //مدير مديرية الحسابات
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.AccountsDirectorate && u.position == Positions.DirectorateHead && u.organization == 2);
                userNameDB[12] = users.username;
                //رئيس قسم النفقات والمخصصات
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.ExpenseSection && u.position == Positions.sectionHead && u.organization == 2);
                userNameDB[13] = users.username;
                //المحاسب
                //userNameDB[14] = contractorStaff.Accountant;
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
                RequestSender = RequestSender,
            };
            context.Output = infoX;
            return Done();
        }
    }
}

