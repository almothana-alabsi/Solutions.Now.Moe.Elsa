using System.Threading.Tasks;
using System.Threading;
using Fluid;
using MediatR;
using Solutions.Now.Moe.Elsa.Models;
using Elsa.Scripting.Liquid.Messages;
using Solutions.Now.Moe.Elsa.Models.Construction.DTOs;

namespace Solutions.Now.Moe.Elsa.Handlers
{
    public class ConfigureLiquidEngine : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            notification.TemplateContext.Options.MemberAccessStrategy.Register<OutputActivityData>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<DataForRequestProject>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<Activities.AssignedEmployee>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<StagesDataForRequestProject>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<projectsTenderFlagsDTO>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<directOrderToTheContractorDTO>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<raiseSurveyorsDTO>();
            notification.TemplateContext.Options.MemberAccessStrategy.Register<changeOrderDTO>();

            notification.TemplateContext.Options.MemberAccessStrategy.Register<string>();
            return Task.CompletedTask;
        }
    }
}
