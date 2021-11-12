using Autofac;
using Yin.Application.Behaviors;
using Yin.Application.DomainEventHandlers;
using MediatR;
using System.Reflection;

namespace Yin.HttpCommon.AutoFacDI
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            // 注册所有Command CommandHandle
            //builder.RegisterAssemblyTypes(typeof(DefaultCommandHandler).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IRequestHandler<,>));


            // 注册 INotification INotificationHandle
            builder.RegisterAssemblyTypes(typeof(DefaultDomainEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));


            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { return componentContext.TryResolve(t, out object o) ? o : null; };
            });


            // 注册 Command Validators 

            //builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            // command 暂时  不做验证
            // builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}
