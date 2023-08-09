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
using NetTopologySuite.GeometriesGraph;
using Positions = Solutions.Now.Moe.Elsa.Common.Positions;
using System.Runtime.Intrinsics.Arm;
using Solutions.Now.Moe.Elsa.Models.Construction;
using NetBox.Extensions;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "Construction_Formationofacommittee",
          Description = "Construction_Formationofacommittee in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_Formationofacommittee : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_Formationofacommittee(ConstructionDBContext constructionDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = constructionDBContext;
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
                List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == 4812).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();


               List<Construction_CommitteeMember> committeeMembers = _ConstructionDBContext.CommitteeMember.AsQueryable().Where(x=>x.projectSerial == 6).ToList<Construction_CommitteeMember>();
 

                foreach (var member in committeeMembers)
                {
                    committeemember.Add(member.userName);
                    steps.Add(r);
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
            
                name = committeemember,
           
            };
            context.Output = infoX;
            return Done();
        }
    }
}

