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
using Amazon.CloudSearchDomain.Model;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "CommitteeStudyDesign",
          Description = "CommitteeStudyDesign",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class CommitteeStudyDesignProject : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        public CommitteeStudyDesignProject(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;
        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]


        public int RequestSerial { get; set; }

        [ActivityInput(Hint = "Enter a Major", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]

        public int Major { get; set; }
        //[ActivityInput(Hint = "Enter a Step", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]

        //public int Step { get; set; }
        //[ActivityInput(Hint = "Enter a Screen", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]

        //public string Screen { get; set; }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screens = new List<string>();
            TblUsers users;
            Committee committee;

            try
            {
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == 4764).OrderBy(s => s.step).ToList<WorkFlowRules>();

                committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.ProjectSerial == RequestSerial);

                List<CommitteeMember> committeeMembers = _moeDBContext.CommitteeMember.AsQueryable().Where(s => s.committeeSerial == committee.Serial && s.major == Major).ToList<CommitteeMember>();

                foreach (var member in committeeMembers)
                {
                    userNameDB.Add(member.userName);

                }


            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            DataForRequestProject infoX = new DataForRequestProject
            {
                requestSerial = RequestSerial,
                name = userNameDB.ToList<string>()

            };
            context.Output = infoX;
            return Done();
        }
    }
}
