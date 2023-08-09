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
using System.Xml.Linq;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
        Category = "Approval",
        DisplayName = "getQuantitativeCalculator",
        Description = "getQuantitativeCalculator",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class getQuantitativeCalculator : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        public getQuantitativeCalculator(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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

                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == 4765).OrderBy(s => s.step).ToList<WorkFlowRules>();

                for (int i = 0; i < workFlowRules.Count; i++)
                {
                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screens.Add(workFlowRules[i].screen);
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            context.Output = userNameDB[6];
            return Done();
        }
    }
}