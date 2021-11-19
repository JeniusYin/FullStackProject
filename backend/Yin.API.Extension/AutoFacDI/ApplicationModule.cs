using Autofac;
using Yin.Domain.Interfaces;
using Yin.EntityFrameworkCore.Repositories;
using Yin.Infrastructure.Redis;
using System;
using System.Linq;
using Yin.Application.Services;
using Yin.Application.Contracts.AdminUser;

namespace Yin.API.Extension.AutoFacDI
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region Repository
            builder.RegisterAssemblyTypes(new[] { typeof(AdminUserRepository).Assembly, typeof(IAdminUserRepository).Assembly})
                   .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null
                           && t.Namespace.EndsWith(".EntityFrameworkCore.Repositories", StringComparison.CurrentCultureIgnoreCase)
                           && t.FullName.EndsWith("Repository", StringComparison.CurrentCultureIgnoreCase)
                           && !t.FullName.EndsWith("BaseRepository", StringComparison.CurrentCultureIgnoreCase))
                   .AsImplementedInterfaces()
                   .InstancePerDependency();
            #endregion

            #region Service
            builder.RegisterAssemblyTypes(new[] { typeof(AdminUserService).Assembly, typeof(IAdminUserService).Assembly })
                   .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null
                           && t.Namespace.EndsWith(".Application.Services", StringComparison.CurrentCultureIgnoreCase)
                           && t.FullName.EndsWith("Service", StringComparison.CurrentCultureIgnoreCase))
                   .AsImplementedInterfaces()
                   .InstancePerDependency();
            #endregion

            #region Config

            builder.RegisterType<RedisHelper>()
                .As<IRedisHelper>()
                .SingleInstance();

            builder.RegisterType<RedisLockKey>()
                .As<IRedisLockKey>()
                .SingleInstance();

            #endregion
        }
    }
}
