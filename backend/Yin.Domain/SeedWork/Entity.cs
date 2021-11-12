using MediatR;
using System;
using System.Collections.Generic;

namespace Yin.Domain.SeedWork
{
    public abstract class Entity : IEntity
    {
        int? _requestedHashCode;
        Guid _Id;
        public virtual Guid Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; protected set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; protected set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; protected set; }


        protected virtual void Init()
        {
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
            IsDelete = false;
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
            return this.Id == default(Guid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }


    public abstract class Entity<T> : IEntity where T : struct
    {
        int? _requestedHashCode;
        T _Id;
        public virtual T Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; protected set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; protected set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; protected set; }


        protected virtual void Init()
        {
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
            IsDelete = false;
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
            return this.Id.Equals(default(T));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<T>))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            var item = (Entity<T>)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return this.Id.Equals(default(T));
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(Entity<T> left, Entity<T> right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity<T> left, Entity<T> right)
        {
            return !(left == right);
        }
    }
}
