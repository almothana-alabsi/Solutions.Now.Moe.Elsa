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
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "CommitteeApproval",
          Description = "Add approval in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class CommitteeApproval : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public CommitteeApproval(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<string> EmailDB = new List<string>();
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screens = new List<string>();
            List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Committee   && s.type == WorkFlowType.WorkflowType).OrderBy(s => s.step).ToList<WorkFlowRules>();
            TblUsers users;
            for (int i = 0; i < workFlowRules.Count; i++)
            {

                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screens.Add(workFlowRules[i].screen);
            }

            try
            {
                Committee committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.Serial == RequestSerial);
                ReferedTender referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(i => i.Serial == committee.TenderSerial);

                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.section && u.position == Positions.sectionHead);
                userNameDB[0] = users.username;
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead);
                userNameDB[1] = users.username;
                //users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead);
                //userNameDB[2] = users.username;
                //users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SG);
                //userNameDB[3] = users.username;
                //if (committee.TenderSerial != null)
                //{
                //    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Consultant == referedTender.Consultant);
                //    userNameDB[2] = users.username;

                //}
                //    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(x => x.position == Positions.Architect );
                //if (users != null)
                //{
                //    userNameDB[2] = users.username;
                //}



                //var committeeMemberArchitectural = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.committeeSerial == committee.Serial && c.major == Positions.Architect);
                //if (committeeMemberArchitectural != null)
                //{
                //    userNameDB[2] = committeeMemberArchitectural.userName;
                //}
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
                Screens = Screens,
            };
            context.Output = infoX;
            return Done();
        }
    }
}

