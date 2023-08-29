using Amazon.AWSSupport.Model;
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
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "FinancalPayment",
          Description = "Add approval in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Financialpayment : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        public Financialpayment(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
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
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.FinancialpaymenttoCounsultant && s.type == WorkFlowType.WorkflowType).OrderBy(s => s.step).ToList<WorkFlowRules>();
                FinancialRequest financialRequest = await _moeDBContext.FinancialRequest.FirstOrDefaultAsync(i => i.serial == RequestSerial);
                ReferedTender referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(s => s.Serial.ToString().Equals(financialRequest.tenderSerial.ToString()));
                ReferedTender refTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(r => r.Serial == financialRequest.tenderSerial);
                Committee committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.TenderSerial == referedTender.Serial);
                CommitteeMember committeeMember = await _moeDBContext.CommitteeMember.FirstOrDefaultAsync(i =>  i.committeeSerial == committee.Serial && i.capten == 1 );
                for (int i = 0; i < workFlowRules.Count; i++)
                {

                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }
                //consultant
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Consultant == refTender.Consultant && u.position == Hierarchy.Consultant);
                userNameDB[0] = users.username;
                //architect
                //CommitteeMember com = _moeDBContext.CommitteeMember.AsQueryable().FirstOrDefault(c => c.capten == 1);
                userNameDB[1] = committeeMember.userName;
                //section Head
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.section && u.position == Positions.sectionHead);
                userNameDB[2] = users.username;
                //Engineering Affairs Directorate
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead);
                userNameDB[3] = users.username;
                //Administration ManagerADMI
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead);
                userNameDB[4] = users.username;
                //Secretary General
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u =>  u.position == Positions.SG);
                userNameDB[5] = users.username;
                //AdminstratorFinancial 
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.AdministrationHead);
                userNameDB[6] = users.username;
                //Accounts Directorate
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.DirectorateHead);
                userNameDB[7] = users.username;
                //Expenses Head
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.AdminstratorFinancial && u.position == Positions.sectionHead);
                userNameDB[8] = users.username;
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

