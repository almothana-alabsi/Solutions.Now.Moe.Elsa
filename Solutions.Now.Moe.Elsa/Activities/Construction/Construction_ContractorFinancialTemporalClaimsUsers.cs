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

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
          Category = "Construction",
          DisplayName = "Construction Contractor Financial Temporal Claims approval",
          Description = "Construction Contractor Financial Temporal Claims in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_ContractorFinancialTemporalClaimsUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_ContractorFinancialTemporalClaimsUsers(ConstructionDBContext ConstructionDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_ContractorFinancialTemporalClaims).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

            for (int i = 0; i < workFlowRules.Count; i++)
            {

                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screen.Add(workFlowRules[i].screen);
            }

            try
            {
                //   var contractorStaff = await _ConstructionDBContext.TenderAdvancePaymentRequest.FirstOrDefaultAsync(x => x.serial == RequestSerial);
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
                //المهندس المشرف
                var committeeCaptain = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.projectSerial == RequestSerial && x.type == WorkFlowsName.Construction_SupervisionCommittee && x.captain == 1);
                userNameDB[2] = committeeCaptain.userName;
                ////مدير الشؤون الادارية والمالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Directorate == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.position == Positions.DirectorateHead);
                if (users != null)
                {
                    userNameDB[3] = users.username;
                }
              
           
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead);
                if (users != null)
                {
                    userNameDB[4] = users.username;
                }


                //رئيس قسم متابعة تنفيذ المشاريع المحلية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.Section == Hierarchy.sectionOfFollowUpToImplementationOfLocalProjectsSection && u.position == Positions.sectionHead && u.organization == 2);
                userNameDB[6] = userNameDB[11]= userNameDB[12]= users.username;
                //مهندس اتصال
                var CommunicationEng = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial && x.type == WorkFlowsName.Construction_CommunicationEng && x.captain == 1);
                userNameDB[5] = userNameDB[13]= CommunicationEng.userName;
                //مدير مديرية الشؤون الهندسية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead && u.organization == 2);
                userNameDB[7] =userNameDB[14] = users.username;
                //مدير ادارة الابنية والمشاريع الدولية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead && u.organization == 2);
                userNameDB[8] = userNameDB[15]=users.username;
                //الامين العام للشؤون الادارية والمالية 
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SecretaryGeneralMoe && u.organization == 2);
                userNameDB[9] = userNameDB[16] = users.username;



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

