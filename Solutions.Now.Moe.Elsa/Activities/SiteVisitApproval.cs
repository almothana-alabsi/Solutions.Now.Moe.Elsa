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
          DisplayName = "SiteVisitApproval",
          Description = "Add approval in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class SiteVisitApproval : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public SiteVisitApproval(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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
            List<string> Screen = new List<string>();
            TblUsers users;

           try
            { 
                 List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.siteVisit && s.type == WorkFlowType.WorkflowType).OrderBy(s => s.step).ToList<WorkFlowRules>();

                for (int i = 0; i < workFlowRules.Count; i++)
                {
                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }
                    SiteVisit siteVisit = await _moeDBContext.SiteVisit.FirstOrDefaultAsync(i => i.Serial == RequestSerial);
                    ReferedTender referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(r => r.Serial == siteVisit.tenderSerial);

                    userNameDB[0] = siteVisit.RequestBy;
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.section && u.position == Positions.sectionHead);
                    userNameDB[1] = users.username;
                    users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead);
                    userNameDB[2] = users.username;

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
                    };
            context.Output = infoX;
            return Done();
        }
    }
}

