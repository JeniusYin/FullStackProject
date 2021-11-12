using System;
using System.Threading.Tasks;

namespace Yin.EntityFrameworkCore.Context
{
    public interface IDbManager
    {
        Task ExecuteSql(FormattableString sql);

        Task ExecuteSqlRaw(string sql);
    }
}
