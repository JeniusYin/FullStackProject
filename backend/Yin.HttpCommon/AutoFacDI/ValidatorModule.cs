﻿
using Autofac;
using FluentValidation;

namespace Yin.HttpCommon.AutoFacDI
{
    public class ValidatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterAssemblyTypes(typeof(DemoRequestValidator).Assembly)
            //    .Where(t => t.IsClosedTypeOf(typeof(AbstractValidator<>)))
            //    .AsClosedTypesOf(typeof(IValidator<>))
            //    .InstancePerDependency();
        }
    }
}
