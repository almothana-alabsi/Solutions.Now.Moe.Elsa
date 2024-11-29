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
using System;
using Solutions.Now.Moe.Elsa.Models.Construction;
using System.Linq;
using Solutions.Now.Moe.Elsa.Common;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
       Category = "Construction",
       DisplayName = "   Construction_InitialReceipt",
       Description = "Construction_InitialReceipt in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class Construction_InitialReceipt : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_InitialReceipt(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Sender.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string RequestSender { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Type.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestType { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_InitialReceipt).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

                for (int i = 0; i < workFlowRules.Count; i++)
                {

                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }

            try
            {
                var initialreceipt = await _ConstructionDBContext.TakeOver.FirstOrDefaultAsync(x => x.serial == RequestSerial);
                var tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(x => x.tenderSerial == initialreceipt.tenderSerial);
                //المقاول
                userNameDB[0] = RequestSender;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.contractor == tender.tenderContracter1 && u.position == Positions.Contractor);
                if (users != null)
                {
                    userNameDB[17] = userNameDB[18] = users.username;
                }
                //رئيس اللجنة
                var committeeCaptain = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial && x.type == WorkFlowsName.Construction_SupervisionCommittee && x.captain == 1);
                if (committeeCaptain != null)
                {
                    userNameDB[1] = userNameDB[12]= userNameDB[19] = committeeCaptain.userName;
                }
                //رئيس قسم الابنية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.sectionBuilding && u.position == Positions.sectionHead && u.organization == Organization.MOE);
                if (users != null)
                {
                    userNameDB[2] = userNameDB[13]= users.username;
                }
                //مدير الشؤون الادارية والمالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Directorate == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.position == Positions.DirectorateHead);
                if (users != null)
                {
                    userNameDB[3] = userNameDB[14] = users.username;
                }
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead && (u.organization == Organization.MOE || u.organization == 3));
                if (users != null)
                {
                    userNameDB[4] = userNameDB[15] = users.username;
                }
                //رئيس قسم متابعة تنفيذ المشاريع المحلية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.Section == Hierarchy.sectionOfFollowUpToImplementationOfLocalProjectsSection && u.position == Positions.sectionHead && u.organization == 2);
                userNameDB[5] = users.username;
                //مهندس اتصال

                var CommunicationEng = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial && x.type == WorkFlowsName.Construction_CommunicationEng && x.captain == 1);
                if (CommunicationEng != null)
                {
                    userNameDB[6] = CommunicationEng.userName;

                }
                //رئيس قسم متابعة تنفيذ المشاريع المحلية

                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.Section == Hierarchy.sectionOfFollowUpToImplementationOfLocalProjectsSection && u.position == Positions.sectionHead && u.organization == 2);
                if (users != null)
                {
                    userNameDB[7] = users.username;
                }
                //مدير مديرية الشؤون الهندسية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead && u.organization == 2);
                if (users != null)
                {
                    userNameDB[8] = users.username;
                }
                //مدير ادارة الابنية والمشاريع الدولية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead && u.organization == 2);
                if (users != null)
                {
                    userNameDB[9] = users.username;
                }
                //الامين العام للشؤون الادارية والمالية 
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SecretaryGeneralMoe && u.organization == 2);
                if (users != null)
                {
                    userNameDB[10] = users.username;

                }
                // رئيس اللجنة للمشروع
                var CaptinCommittee = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial &&x.projectSerial==initialreceipt.projectSerial&& x.type == WorkFlowsName.Construction_InitialReceipt && x.captain == 1);
                if (CaptinCommittee != null)
                {
                    userNameDB[11]  = userNameDB[16] = CaptinCommittee.userName;
                }

                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.contractor == tender.tenderContracter1 && u.position == Positions.Contractor);
                if (users != null)
                {
                    userNameDB[17] = userNameDB[18] = users.username;
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
                RequestSender = RequestSender,
                requestType = RequestType

            };
            context.Output = infoX;
            return Done();
        }
    }
}
