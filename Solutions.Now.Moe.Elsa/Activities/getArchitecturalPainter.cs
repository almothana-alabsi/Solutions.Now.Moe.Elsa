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
using Microsoft.EntityFrameworkCore;

namespace Solutions.Now.Moe.Elsa.Activities
{
    [Activity(
        Category = "Approval",
        DisplayName = "getArchitecturalPainter",
        Description = "getArchitecturalPainter",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class getArchitecturalPainter : Activity
    {
        private readonly DesignReviewDBContext _DesignReviewDBContext;
        private readonly SsoDBContext _ssoDBContext;
        private readonly MoeDBContext _moeDBContext;
        public getArchitecturalPainter(DesignReviewDBContext DesignReviewDBContext, SsoDBContext ssoDBContext, MoeDBContext moeDBContext)
        {
            _DesignReviewDBContext = DesignReviewDBContext;
            _ssoDBContext = ssoDBContext;
            _moeDBContext = moeDBContext;

        }
        [ActivityInput(Hint = "Enter an expression that evaluates to the Request serial.", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]


        public int RequestSerial { get; set; }

        [ActivityInput(Hint = "Enter a Major", DefaultSyntax = SyntaxNames.Literal, SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]

        public int Major { get; set; }



        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            List<int?> steps = new List<int?>();
            List<string> userNameDB = new List<string>();
            List<string> Screens = new List<string>();
            TblUsers users;
            Committee committee;
            int x = 0;

            try
            {

                committee = await _moeDBContext.Committee.FirstOrDefaultAsync(i => i.ProjectSerial == RequestSerial);

                List<CommitteeMember> committeeMembers = _moeDBContext.CommitteeMember.AsQueryable().Where(s => s.committeeSerial == committee.Serial && s.major == Major).ToList<CommitteeMember>();

                if (committeeMembers.Count > 0) { x = 1; };

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
 
            context.Output = x;
            return Done();
        }
    }
}