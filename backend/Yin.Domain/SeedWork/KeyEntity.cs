using MediatR;
using System;
using System.Collections.Generic;
using Yin.Infrastructure.GuidGenerator;

namespace Yin.Domain.SeedWork
{
    public abstract class KeyEntity
    {
        public virtual Guid Id { get; protected set; }

        protected virtual void Init()
        {
            Id = GuidGenerateHelper.GenerateSequenceGuid();
        }

        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return this.Id.Equals(default);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not KeyEntity)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            var item = (KeyEntity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return this.Id.Equals(default);
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                return Id.GetHashCode() ^ 31;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(KeyEntity left, KeyEntity right)
        {
            if (Equals(left, null))
                return Equals(right, null);
            else
                return left.Equals(right);
        }

        public static bool operator !=(KeyEntity left, KeyEntity right)
        {
            return !(left == right);
        }
    }
}
