﻿using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Services;
using Elsa.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elsa.Expressions;
using System;
using NodaTime;
using Elsa.Builders;
using Elsa.Activities.Temporal;
using Solutions.Now.Moe.Elsa.Models.Construction;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Common;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.CMIS2.Elsa.Activities
{
    [Activity(
          Category = "Approval",
          DisplayName = "Defects Liability Contractor Duration Partial approvals",
          Description = "Add Users of Defects Liability Contractor Duration1 Partial approvals",
          Outcomes = new[] { OutcomeNames.Done }
      )]
    public class Construction_DefectsLiabilityContractorDurationPartial : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        private readonly IClock _clock;

        public Construction_DefectsLiabilityContractorDurationPartial(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext, IClock clock)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;
            _clock = clock;
        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        public int? durations { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {

            IList<int?> steps = new List<int?>();
            IList<string> userNameDB = new List<string>();
            IList<string> forms = new List<string>();

            List<WorkFlowRulesConstruction> workFlowRules = _ConstructionDBContext.WorkFlowRules.AsQueryable().Where(s => s.workflow == WorkFlowsName.Construction_InitialReceipt).OrderBy(s => s.step).ToList<WorkFlowRulesConstruction>();
            TblUsers users;

            for (int i = 0; i < workFlowRules.Count; i++)
            {
                userNameDB.Add(workFlowRules[i].username);
                steps.Add(workFlowRules[i].step);
                forms.Add(workFlowRules[i].screen);
            }
            try
            {
                var partialTakeOver = await _ConstructionDBContext.partialTakeOver.FirstOrDefaultAsync(x => x.serial == RequestSerial);
                var tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(x => x.tenderSerial == partialTakeOver.tenderSerial);
                //المقاول
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.contractor == tender.tenderContracter1 && u.position == Positions.Contractor);
                if (users != null)
                {
                    userNameDB[7] = userNameDB[12] = users.username;
                }
                //المهندس المشرف
                var committeeCaptain = await _ConstructionDBContext.CommitteeMember.FirstOrDefaultAsync(x => x.tenderSerial == tender.tenderSerial && x.type == WorkFlowsName.Construction_SupervisionCommittee && x.captain == 1);
                if (users != null)
                {
                    userNameDB[8] = userNameDB[13] = committeeCaptain.userName;

                }//رئيس قسم الابنية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Section == Hierarchy.sectionBuilding && u.position == Positions.sectionHead && u.organization == Organization.MOE);
                if (users != null)
                {
                    userNameDB[9] = userNameDB[14] = users.username;
                }
                //مدير الشؤون الادارية والمالية
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.Directorate == Hierarchy.DirectorateOfAdministrativeAndFinancialAffairs && u.position == Positions.DirectorateHead);
                if (users != null)
                {
                    userNameDB[10] = userNameDB[15] = users.username;
                }
                //مدير مديرية التربية والتعليم
                users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.Administration == tender.tenderSupervisor && u.position == Positions.AdministrationHead && u.organization == Organization.MOE);
                if (users != null)

                {
                    userNameDB[11] = userNameDB[16] = users.username;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            context.Output = durations;
            return Done();
        }

    }
}