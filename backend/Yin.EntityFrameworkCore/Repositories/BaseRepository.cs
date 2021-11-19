using Yin.Domain.Interfaces;
using Yin.Domain.SeedWork;
using Yin.EntityFrameworkCore.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Yin.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// 通用仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        public IUnitOfWork UnitOfWork { get; }
        public MyDbContext Context { get; }
        private DbSet<TEntity> DbSet { get; }

        public BaseRepository(MyDbContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
            UnitOfWork = context;
        }

        public async Task Add(TEntity obj)
        {
            await DbSet.AddAsync(obj);
        }

        public async Task AddRange(List<TEntity> list)
        {
            await DbSet.AddRangeAsync(list);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return DbSet;
        }

        public async Task<bool> Exist(Expression<Func<TEntity, bool>> express)
        {
            return await DbSet.AsNoTracking().AnyAsync(express);
        }

        public async Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> express, params string[] includes)
        {
            return await GetNavigation(false, includes).Where(express).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllWithNoTracking(Expression<Func<TEntity, bool>> express,
            params string[] includes)
        {
            return await GetNavigation(true, includes).AsNoTracking().Where(express).ToListAsync();
        }

        public async Task<TEntity> GetFirstOrDefault(Expression<Func<TEntity, bool>> express, params string[] includes)
        {
            return await GetNavigation(false, includes).Where(express).FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetFirstOrDefaultNoTracking(Expression<Func<TEntity, bool>> express, params string[] includes)
        {
            return await GetNavigation(true, includes).Where(express).AsNoTracking().FirstOrDefaultAsync();
        }

        private IQueryable<TEntity> GetNavigation(bool isReadOnly, params string[] includes)
        {
            var dbSetTemp = isReadOnly ? DbSet.AsNoTracking() : DbSet;
            if (includes?.Any() != true)
            {
                return dbSetTemp;
            }
            else
            {
                var dbSet = dbSetTemp.Include(includes[0]);
                for (int i = 1; i < includes.Length; i++)
                {
                    // 存在include 失败的情况
                    dbSet = dbSetTemp.Include(includes[i]);
                }

                return dbSet;
            }
        }
    }
}
