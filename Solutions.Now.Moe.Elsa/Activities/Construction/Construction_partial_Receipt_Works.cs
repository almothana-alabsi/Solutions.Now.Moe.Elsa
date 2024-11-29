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
       DisplayName = "  Construction_partial_Receipt_Works",
       Description = "Construction_InitialReceipt in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class Construction_partial_Receipt_Works : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_partial_Receipt_Works(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
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
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_partial_Receipt_Works).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

            for (int i = 0; i < workFlowRules.Count; i++)
            {

                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screen.Add(workFlowRules[i].screen);
            }

            try
            {
                var partialTakeOver = await _ConstructionDBContext.partialTakeOver.FirstOrDefaultAsync(x => x.serial == RequestSerial);
                var tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(x => x.tenderSerial == partialTakeOver.tenderSerial);
                //رئيس قسم المتابعة 


                userNameDB[0] = RequestSender;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.contractor == tender.tenderContracter1 && u.position == Positions.Contractor);
                if (users != null)
                {
                    userNameDB[7] = userNameDB[12] = users.username;
                }
                //مهندس اتصال
                var CommunicationEng = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial && x.type == WorkFlowsName.Construction_CommunicationEng && x.captain == 1);
                if (CommunicationEng != null)
                {
                    userNameDB[1] = CommunicationEng.userName;
                } //رئيس قسم متابعة تنفيذ المشاريع المحلية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.Section == Hierarchy.sectionOfFollowUpToImplementationOfLocalProjectsSection && u.position == Positions.sectionHead && u.organization == 2);
                if (users != null)
                {
                    userNameDB[2] = users.username;
                }//مدير مديرية الشؤون الهندسية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead && u.organization == 2);
                if (users != null)
                {
                    userNameDB[3] = users.username;
                } //مدير ادارة الابنية والمشاريع الدولية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead && u.organization == 2);
                if (users != null)
                {
                    userNameDB[4] = users.username;
                }  //الامين العام للشؤون الادارية والمالية 
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SecretaryGeneralMoe && u.organization == 2);
                if (users != null)
                {
                    userNameDB[5] = users.username;

                }// رئيس اللجنة للمشروع
                var CaptinCommittee = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial && x.type == WorkFlowsName.Construction_partial_Receipt_Works && x.captain == 1);
                if (CaptinCommittee != null)
                {
                    userNameDB[6] = userNameDB[17] = CaptinCommittee.userName;
                }

                //المقاول
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.contractor == tender.tenderContracter1 && u.position == Positions.Contractor);
                if (users != null)
                {
                    userNameDB[7] =userNameDB[12]= users.username;
                }
                //المهندس المشرف
                var committeeCaptain = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial && x.type == WorkFlowsName.Construction_SupervisionCommittee && x.captain == 1);
                if (committeeCaptain != null)
                {
                    userNameDB[8] = userNameDB[13] = committeeCaptain.userName;

                }//رئيس قسم الابنية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.sectionBuilding && u.position == Positions.sectionHead && u.organization == Organization.MOE);
                if (users != null)
                {
                    userNameDB[9] = userNameDB[14] = users.username;
                }
                //مدير الشؤون الادارية والمالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Directorate == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.position == Positions.DirectorateHead);
                if (users != null)
                {
                    userNameDB[10]=userNameDB[15] = users.username;
                }
               
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead && (u.organization == Organization.MOE || u.organization == 3));
                if (users != null)

                {
                    userNameDB[11] = userNameDB[16] = users.username;
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
