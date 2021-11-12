using Yin.Domain.SeedWork;
using Yin.EntityFrameworkCore.Context;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

namespace Yin.EntityFrameworkCore.Extensions
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, MyDbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var e in domainEvents)
            {
                await mediator.Publish(e);
            }
        }
    }
}
