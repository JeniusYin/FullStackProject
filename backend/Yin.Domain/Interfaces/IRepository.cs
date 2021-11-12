using Yin.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Yin.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        /// <summary>
        /// UnitOfWork
        /// </summary>
        IUnitOfWork UnitOfWork { get; }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        Task Add(TEntity obj);
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="list"></param>
        Task AddRange(List<TEntity> list);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();
        /// <summary>
        /// GetFirstOrDefaultNoTracking
        /// </summary>
        /// <param name="express"></param>
        /// <param name="includes">导航属性</param>
        /// <returns></returns>
        Task<TEntity> GetFirstOrDefaultNoTracking(Expression<Func<TEntity, bool>> express, params string[] includes);
        /// <summary>
        /// GetFirstOrDefault
        /// </summary>
        /// <param name="express"></param>
        /// <param name="includes">导航属性</param>
        /// <returns></returns>
        Task<TEntity> GetFirstOrDefault(Expression<Func<TEntity, bool>> express, params string[] includes);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="express"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> express, params string[] includes);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="express"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllWithNoTracking(Expression<Func<TEntity, bool>> express, params string[] includes);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        Task<bool> Exist(Expression<Func<TEntity, bool>> express);
    }
}
