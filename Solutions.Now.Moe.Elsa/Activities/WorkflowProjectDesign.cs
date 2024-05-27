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
using Amazon.IdentityManagement.Model;
using MassTransit.RabbitMqTransport;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
        Category = "Approval",
        DisplayName = "WorkflowProjectDesign",
        Description = "WorkflowProjectDesign",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class WorkflowProjectDesign : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        public WorkflowProjectDesign(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            int major = 3640;
            List<string> EmailDB = new List<string>();
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            try
            {

                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.DesignReview_WorkflowProjectDesign).OrderBy(s => s.step).ToList<WorkFlowRules>();

                var projects = await _moeDBContext.ProjectsTender.FirstOrDefaultAsync(i => i.Serial == RequestSerial);
                var committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.ProjectSerial == RequestSerial);
                for (int i = 0; i < workFlowRules.Count; i++)
                {
                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }
                TblUsers users = null;
                var committeeMemberArchitectural = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.Architect);
                userNameDB[1] = userNameDB[10] = userNameDB[12] = userNameDB[13] = committeeMemberArchitectural.userName;
                var committeeMemberCivilEngineer = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.CivilEngineer);
                userNameDB[7] = committeeMemberCivilEngineer.userName;
                var committeeMemberElectricalEngineer = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.ElectricalEngineer);
                userNameDB[4] = committeeMemberElectricalEngineer.userName;
                var committeeMemberMechanicalEngineer = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.MechanicalEngineer);
                userNameDB[5] = committeeMemberMechanicalEngineer.userName;
                var committeeMemberSurveyEng = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.SurveyEng);
                if (committeeMemberSurveyEng != null)
                {
                    userNameDB[0] = committeeMemberSurveyEng.userName;
                };
                var committeeMemberArtist = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.ArchitecturalPainter);
                if (committeeMemberArtist != null)
                {
                    userNameDB[3] = committeeMemberArtist.userName;
                }; var committeeMemberQuantitySurveyor = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.QuantitySurveyor);
                if (committeeMemberQuantitySurveyor != null)
                {
                    userNameDB[6] = committeeMemberQuantitySurveyor.userName;
                }
                else 
                {
                    userNameDB[14] = committeeMemberCivilEngineer.userName;
                }
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.section && u.position == Positions.sectionHead);
                userNameDB[2] = userNameDB[11] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead);
                userNameDB[8] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead);
                userNameDB[9] = users.username;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            DataForRequestProject infoX = new DataForRequestProject
            {
                requestSerial = RequestSerial,
                major = major,
                steps = steps,
                name = userNameDB,
                Screens = Screen

            };
            context.Output = infoX;
            return Done();
        }
    }
}