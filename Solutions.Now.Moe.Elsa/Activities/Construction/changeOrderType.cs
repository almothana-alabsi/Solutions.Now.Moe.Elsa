using Elsa;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services.Models;
using Solutions.Now.Moe.Elsa.Models;
using Solutions.Now.Moe.Elsa.Models.Construction;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Solutions.Now.Moe.Elsa.Common;
using Elsa.Services;
using Solutions.Now.Moe.Elsa.Models.Construction.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Elsa.Models;
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities.Construction
{
    [Activity(
     Category = "Construction",
     DisplayName = "change Order Type Approval",
     Description = "Change Order Type Approval in WorkflowRules Table",
     Outcomes = new[] { OutcomeNames.Done }
 )]
    public class changeOrderType:Activity
    {
        private readonly ConstructionDBContext _ConstructionDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
      

        public changeOrderType(ConstructionDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _ConstructionDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;
        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request Serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public int RequestSerial { get; set; }
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            bool resultF = false;
            bool resultT = false;
            bool result = false;
            try
            {
               var change = await _ConstructionDBContext.ChangeOrder.OrderBy(x=>x.serial).FirstOrDefaultAsync(r => r.serial == RequestSerial);
                Tender tender = await _ConstructionDBContext.Tender.FirstOrDefaultAsync(s => s.tenderSerial.Equals(change.tenderSerial));
                var fourth= Convert.ToDecimal((double)tender.tenderAmountUponAssignment *0.15);
                var thirty= Convert.ToDecimal((double)tender.tenderAmountUponAssignment *0.30);
                var amount = change.ChangeOrderAmount;
            if (amount <= fourth)
                {resultF = true;}
            else if (amount > fourth && amount <= thirty)
                {resultT = true;}
            else { result = true; }
        }
       catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message.ToString());
            }
            changeOrderTypeDto infoX = new changeOrderTypeDto
            {
                resultF= resultF,
                resultT = resultT,
                result = result,   
            };
            context.Output = infoX;
            return Done();
        }
    }
    
}
