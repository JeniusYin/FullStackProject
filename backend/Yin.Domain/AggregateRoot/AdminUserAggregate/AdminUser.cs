using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.SeedWork;

namespace Yin.Domain.AggregateRoot.AdminUserAggregate
{
    public partial class AdminUser : RecordableEntity, IAggregateRoot
    {
        public string Account { get; private set; }
        public string Password { get; private set; }
        public string Name { get; private set; }
        public int Status { get; private set; }
        public DateTime LastLoginSuccessTime { get; private set; }
        public DateTime LastLoginErrorTime { get; private set; }
        public int LoginErrorCount { get; private set; }
    }

    public partial class AdminUser
    {
        public AdminUser(string account, string password, string name = null)
        {
            base.Init();
            Account = account;
            Password = password;
            Name = name ?? account;
            Status = 1;
        }
    }
}
