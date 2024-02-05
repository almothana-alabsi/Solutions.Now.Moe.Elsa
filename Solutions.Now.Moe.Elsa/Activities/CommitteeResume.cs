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
using Solutions.Now.Moe.Elsa.Models;
using System;
using Solutions.Now.Moe.Elsa.Common;
using Positions = Solutions.Now.Moe.Elsa.Common.Positions;
using System.Runtime.Intrinsics.Arm;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "CommitteeResume",
          Description = "Add approval in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class CommitteeResume : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public CommitteeResume(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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
            List<string> Screens = new List<string>();
            List<string> committeemember = new List<string>();
            TblUsers users;
            int r = 0;
            Committee committee;

            try
            {
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.ResumeProject).OrderBy(s => s.step).ToList<WorkFlowRules>();
                ResumeProjectWork resumeProject = await _moeDBContext.ResumeProjectWork.FirstOrDefaultAsync(i => i.Serial == RequestSerial);
                if (resumeProject.ProjectSerial == null)
                {
                    committee = await _moeDBContext.Committee.FirstOrDefaultAsync(u => u.TenderSerial == resumeProject.TenderSerial);

                }
                else
                {
                    committee = await _moeDBContext.Committee.FirstOrDefaultAsync(u => u.TenderSerial == resumeProject.TenderSerial && u.ProjectSerial == resumeProject.ProjectSerial);

                }
                List<CommitteeMember> committeeMembers = _moeDBContext.CommitteeMember.AsQueryable().Where(s => s.committeeSerial == committee.Serial).ToList<CommitteeMember>();

                foreach (var member in committeeMembers)
                {
                    committeemember.Add(member.userName);
                    steps.Add(r+7);
                    Screens.Add(workFlowRules[0].screen);
                    r++;
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
                name = committeemember,
                Screens = Screens,

            };
            context.Output = infoX;
            return Done();
        }
    }
}

