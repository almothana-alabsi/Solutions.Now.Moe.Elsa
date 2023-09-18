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
using Solutions.Now.Moe.Elsa.Models.Construction.DTOs;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
          Category = "Construction",
          DisplayName = "Construction Raise Surveyors users approval",
          Description = "Construction Raise Surveyors users in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_RaiseSurveyorsUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_RaiseSurveyorsUsers(ConstructionDBContext ConstructionDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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
            string survey = "";
            int? flag = 0;
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_RaiseSurveyorsUsers).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

            for (int i = 0; i < workFlowRules.Count; i++)
            {

                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screen.Add(workFlowRules[i].screen);
            }

            try
            {
                var construction_Raise = await _ConstructionDBContext.RaiseSurveyors.FirstOrDefaultAsync(x => x.serial == RequestSerial);
                if (construction_Raise.surveyCommittee != null) { survey = construction_Raise.surveyCommittee; };
                if (construction_Raise.retinalMatchingWithContractualSchemes != null) { flag = construction_Raise.retinalMatchingWithContractualSchemes; };
                var tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(x => x.tenderSerial == construction_Raise.tenderSerial);
          
                //المقاول
                userNameDB[0] = RequestSender;
                //رئيس اللجنة
                var committeeCaptain = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == construction_Raise.tenderSerial && x.type == WorkFlowsName.Construction_SupervisionCommittee && x.captain == 1);    
                userNameDB[1] = committeeCaptain.userName;
                //رئيس قسم الابنية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.sectionBuilding && u.position == Positions.sectionHead && u.organization == Organization.MOE);
                if (users != null)
                {
                    userNameDB[2] = users.username;
                }
                //مدير ادارة الشؤون المالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.AdministrationHead && u.organization == Organization.MOE);
                userNameDB[3] = users.username;
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead && u.organization == Organization.MOE);
                if (users != null)
                {
                    userNameDB[4] = users.username;
                }
                //رئيس قسم متابعة تنفيذ المشاريع المحلية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.Section == Hierarchy.sectionOfFollowUpToImplementationOfLocalProjectsSection && u.position == Positions.sectionHead && u.organization == Organization.MOE);
                userNameDB[5] = userNameDB[7] = users.username;
                //المساح
                userNameDB[6] = survey;
                //مدير مديرية الشؤون الهندسية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead && u.organization == 2);
                userNameDB[8] = users.username;
                //مدير ادارة الابنية والمشاريع الدولية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead && u.organization == 2);
                userNameDB[9] = users.username;
                //الامين العام
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SG && u.organization == Organization.MOE);
                userNameDB[10] = users.username;







            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            raiseSurveyorsDTO infoX = new raiseSurveyorsDTO
            {
                requestSerial = RequestSerial,
                steps = steps,
                name = userNameDB,
                Screens = Screen,
                RequestSender = RequestSender,
                surveyCommittee = survey,
                retinalMatchingWithContractualSchemes = flag

            };
            context.Output = infoX;
            return Done();
        }
    }
}

