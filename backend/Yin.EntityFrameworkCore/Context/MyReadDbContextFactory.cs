using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Yin.EntityFrameworkCore.Context
{
    public class MyReadDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public MyReadDbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MyReadDbContext MyReadDbContext
        {
            get
            {
                var context = _serviceProvider.GetRequiredService<MyReadDbContext>();
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                return context;
            }
        }
    }
}
