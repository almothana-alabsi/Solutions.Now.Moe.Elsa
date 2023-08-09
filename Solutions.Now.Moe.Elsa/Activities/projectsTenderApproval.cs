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
using Esprima;
using Solutions.Now.Moe.Elsa.Common;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "projects Tender stages approval",
          Description = "Add approval in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class projectsTenderApproval : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public projectsTenderApproval(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Stage", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? Stage { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            TblUsers users;


            try
            {
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.DeliveryOfStage && s.type == WorkFlowType.WorkflowType).OrderBy(s => s.step).ToList<WorkFlowRules>();
                var projects = await _moeDBContext.ProjectsTender.FirstOrDefaultAsync(i => i.Serial == RequestSerial && i.stage == Stage);
                var referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(s => s.Serial == projects.tenderSerial);
                var committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.TenderSerial == referedTender.Serial);
                for (int i = 0; i < workFlowRules.Count; i++)
                {

                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Consultant == referedTender.Consultant && u.position == Hierarchy.Consultant);
                userNameDB[0] = userNameDB[11] = userNameDB[12] = userNameDB[13] = userNameDB[14] = userNameDB[15] = userNameDB[18] = users.username;

                var committeeMemberArchitectural = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.Architect);
                userNameDB[1] = committeeMemberArchitectural.userName;

                var committeeMemberCivilEngineer = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.CivilEngineer);
                userNameDB[2] = committeeMemberCivilEngineer.userName;

                var committeeMemberElectricalEngineer = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.ElectricalEngineer);
                userNameDB[3] = committeeMemberElectricalEngineer.userName;

                var committeeMemberMechanicalEngineer = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.MechanicalEngineer);
                userNameDB[4] = committeeMemberMechanicalEngineer.userName;

                var committeeMemberSurveyEng = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.SurveyEng);
                if (committeeMemberSurveyEng != null)
                {
                    userNameDB[5] = committeeMemberSurveyEng.userName;
                };

                var committeeMemberQuantitySurveyor = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.QuantitySurveyor);
                userNameDB[16] = committeeMemberQuantitySurveyor.userName;

                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.section && u.position == Positions.sectionHead);
                userNameDB[6] = userNameDB[7] = userNameDB[8] = userNameDB[9] = userNameDB[6] = userNameDB[10] = userNameDB[17] = users.username;
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
                stage = Stage

            };
            context.Output = infoX;
            return Done();
        }
    }
}

