using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.SeedWork;

namespace Yin.Domain.AggregateRoot.AdminUserAggregate
{
    public class AdminUser : RecordableEntity, IAggregateRoot
    {
        public string Account { get; private set; }
        public string Password { get; private set; }
        public string RealName { get; private set; }
        public int Status { get; private set; }
        public DateTime LastLoginSuccessTime { get; private set; }
        public DateTime LastLoginErrorTime { get; private set; }
        public int LoginErrorCount { get; private set; }
    }
}
