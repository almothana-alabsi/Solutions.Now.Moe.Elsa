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
using Solutions.Now.Moe.Elsa.Models.Construction;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "DesignReview ChangeOrder",
          Description = "DesignReview ChangeOrder in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class DesignReview_ChangeOrder : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        public DesignReview_ChangeOrder(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;
        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Sender.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string RequestSender { get; set; }


        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screen = new List<string>();
            TblUsers users;
            try
            {
                List<WorkFlowRules> workFlowRules = _moeDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.DesignReview_ChangeOrder && s.type == WorkFlowType.WorkflowType).OrderBy(s => s.step).ToList<WorkFlowRules>();
             
                ChangeOrder changeOrder = await _moeDBContext.ChangeOrder.FirstOrDefaultAsync(i => i.Serial == RequestSerial);
                
                var positionUser = _ssoDBContext.TblUsers.FirstOrDefault(u => u.username == RequestSender).position;



                ReferedTender referedTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(s => s.Serial.ToString().Equals(changeOrder.tenderSerial.ToString()));
                 ReferedTender refTender = await _moeDBContext.ReferedTender.FirstOrDefaultAsync(r => r.Serial == changeOrder.tenderSerial);
                // Committee committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.TenderSerial == referedTender.Serial);
                // CommitteeMember committeeMember = await _moeDBContext.CommitteeMember.FirstOrDefaultAsync(i => i.committeeSerial == committee.Serial && i.capten == 1);
                for (int i = 0; i < workFlowRules.Count; i++)
                {

                    userNameDB.Add(workFlowRules[i].username);
                    steps.Add(workFlowRules[i].step);
                    Screen.Add(workFlowRules[i].screen);
                }
                // الاستشاري
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Consultant == referedTender.Consultant && u.position == Positions.Consultant);
                userNameDB[0] = users.username;
                //المعماري
                //users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.Directorate && u.Architecture == Tender.tenderArchitecture && u.position == Positions.Architect);
                //userNameDB[1] = users.username;
                //رئيس قسم الدراسات والتصميم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.section && u.position == Positions.sectionHead);
                userNameDB[2] = users.username;
                //مدير مديرية الشؤون الهندسية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead);
                userNameDB[3] = users.username;
                //مدير مديرية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead);
                userNameDB[4] = users.username;
                //امين العام
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SG);
                userNameDB[5] = users.username;
                //الوزير 
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.Minister);
                userNameDB[6] = users.username;
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

