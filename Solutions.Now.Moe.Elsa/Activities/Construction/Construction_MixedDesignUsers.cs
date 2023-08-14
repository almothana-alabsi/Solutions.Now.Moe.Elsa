using Elsa.Attributes;
using Elsa;
using Elsa.Services;
using Elsa.ActivityResults;
using Elsa.Expressions;
using Elsa.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Models.Construction;
using Solutions.Now.Moe.Elsa.Common;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
          Category = "DTO Training",
          DisplayName = "DTO Training Test MixedDesignUsers",
          Description = "DTO Training Test MixedDesignUsers",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class MixedDesignUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        public MixedDesignUsers(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)


        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_MixedDesign).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();

            // TblUsers users;
            for (int i = 0; i < workFlowRules.Count; i++)
            {
                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screen.Add(workFlowRules[i].screen);
            }

            try
            {/*
                users = await constructionDBContext.TblUsers.FirstOrDefaultAsync(u => u.directorate == Hierarchy.Directorate && u.administration == Hierarchy.Adminstration);
                userNameDB[1] = users.username;

                users = await constructionDBContext.TblUsers.FirstOrDefaultAsync(u => u.administration == 39 && u.directorate==Hierarchy.DirectorateOfAdminstrationAndFinancial);
                userNameDB[2] = users.username;

                */

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            OutputActivityData infoX = new OutputActivityData
            {
                requestSerial = RequestSerial,
                steps = steps,
                names = userNameDB,
                Screens = Screen
            };
            context.Output = infoX;
            return Done();
        }
    }
}