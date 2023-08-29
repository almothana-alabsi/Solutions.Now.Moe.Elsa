﻿using Elsa.Attributes;
using Elsa;
using Solutions.Now.Moe.Elsa.Models;
using Elsa.Services;
using Elsa.ActivityResults;
using Elsa.Expressions;
using Elsa.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Solutions.Now.Moe.Elsa.Models.Construction;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
       Category = "Construction",
       DisplayName = "Communication eng Committee Member Approval", 
       Description = "Communication eng Committee Member Approval in WorkflowRules Table",
       Outcomes = new[] { OutcomeNames.Done }
   )]
    public class Construction_CommitteeMemberUsers : Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;

        public Construction_CommitteeMemberUsers(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }

        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? RequestSerial { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the workflow type.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? workflowType { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Section.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? section { get; set; }
        [ActivityInput(Hint = "Enter an expression that evaluates to the tender Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int? tenderSerial { get; set; }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<string> committeemember = new List<string>();
            HashSet<string> userNameDB = new HashSet<string>();
            try
            {
                if (section == null && tenderSerial == null)
            {
                var committeeMembers = await _ConstructionDBContext.CommitteeMember.AsAsyncEnumerable().Where(c => c.projectSerial == RequestSerial && c.type == workflowType).ToListAsync();
                for (int i = 0; i < committeeMembers.Count; i++)
                {
                    if (!String.IsNullOrEmpty(committeeMembers[i].userName.ToString()))
                    {
                        //TblUsers users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.username == committeeMember[i].userName);
                        committeemember.Add(committeeMembers[i].userName);

                    }

                }
            }
            else if (section != null && tenderSerial == null)
            {
                var committeeMembers = await _ConstructionDBContext.CommitteeMember.AsAsyncEnumerable().Where(c => c.projectSerial == RequestSerial && c.type == workflowType && c.section == section).ToListAsync();

                for (int i = 0; i < committeeMembers.Count; i++)
                {
                    if (!String.IsNullOrEmpty(committeeMembers[i].userName.ToString()))
                    {
                        committeemember.Add(committeeMembers[i].userName);

                    }

                }
            }
                else if (section == null && tenderSerial != null)
                {
                    var sCommittee = await _ConstructionDBContext.SupervisionCommittee.FirstOrDefaultAsync(c => c.serial == tenderSerial);

                    var committeeMembers = await _ConstructionDBContext.CommitteeMember.AsAsyncEnumerable().Where(c => c.tenderSerial == sCommittee.tenderSerial && c.type == workflowType).ToListAsync();
                    for (int i = 0; i < committeeMembers.Count; i++)
                    {
                        if (!String.IsNullOrEmpty(committeeMembers[i].userName.ToString()))
                        {
                            //TblUsers users = await _ssoDBContext.TblUsers.FirstOrDefaultAsync(u => u.username == committeeMember[i].userName);
                            committeemember.Add(committeeMembers[i].userName);

                        }

                    }
                }


            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            DataForRequestProject infoX = new DataForRequestProject
            {
                name = committeemember
            };
            context.Output = infoX;
            return Done();
        }
    }
}
