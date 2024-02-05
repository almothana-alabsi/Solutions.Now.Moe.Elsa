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
using Solutions.Now.Moe.Elsa.Models.Construction;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
          Category = "Construction",
          DisplayName = "Delegate Director Education With Engineer Permissions Users approval",
          Description = "Delegate Director Education With Engineer Permissions Users in ApprovalHistory Table",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_delegateDirectorEducationWithEngineerPermissionsUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_delegateDirectorEducationWithEngineerPermissionsUsers(ConstructionDBContext ConstructionDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = ConstructionDBContext;
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
            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_Delegate).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

            for (int i = 0; i < workFlowRules.Count; i++)
            {

                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                Screen.Add(workFlowRules[i].screen);
            }

            try
            {
                var delegateDirectorateDirectore = await _ConstructionDBContext.DelegateDirectorEducationWithEngineerPermissions.FirstOrDefaultAsync(x => x.serial == RequestSerial);

                var tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(x => x.tenderSerial == delegateDirectorateDirectore.tenderSerial);
                userNameDB[0] = tender.tendersDepartmentEngineer.ToString();
                //رئيس قسم العطاءات
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Section == Hierarchy.sectionOfTender && u.position == Positions.sectionHead && u.organization == 2);
                userNameDB[1] = users.username;
                //مدير ادارو الابنية والمشاريع الدولية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == Hierarchy.Administration && u.position == Positions.AdministrationHead && u.organization == 2);
                userNameDB[2] = users.username;
                //الامين العام
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.position == Positions.SG && u.organization == Organization.MOE);
                userNameDB[3] = users.username;
                //مدير مديرية الصيانة
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.maintenanceDepartment && u.position == Positions.DirectorateHead);
                userNameDB[4] = users.username;
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead);
                if (users != null)
                {
                    userNameDB[5] = users.username;
                }
                //رئيس قسم الابنية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.sectionBuilding && u.position == Positions.sectionHead);
                if (users != null)
                {
                    userNameDB[7] = users.username;
                }     
                //مدير مديرية الشؤون الهندسية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.position == Positions.DirectorateHead && u.organization == 2);
                userNameDB[9] = users.username;
                //رئيس قسم متابعة تنفيذ المشاريع المحلية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Directorate == Hierarchy.Directorate && u.Section == Hierarchy.sectionOfFollowUpToImplementationOfLocalProjectsSection && u.position == Positions.sectionHead && u.organization == 2);
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
                RequestSender = RequestSender

            };
            context.Output = infoX;
            return Done();
        }
    }
}

