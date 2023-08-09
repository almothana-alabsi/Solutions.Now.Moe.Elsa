using System.Threading.Tasks;
using System.Threading;
using Fluid;
using MediatR;
using Solutions.Now.Moe.Elsa.Models;
using Elsa.Scripting.JavaScript.Messages;
using Elsa.Scripting.JavaScript.Options;
using Elsa.Scripting.JavaScript.Extensions;
using Elsa.Scripting.JavaScript.Events;

namespace Solutions.Now.Moe.Elsa.Handlers
{
    public class ConfigureJavaScriptEngine : INotificationHandler<EvaluatingJavaScriptExpression>
    {
        public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
        {
            var engine = notification.Engine;
            engine.RegisterType<OutputActivityData>();
            engine.RegisterType<string>();
            engine.RegisterType<int>();

            return Task.CompletedTask;
        }

    }
}
