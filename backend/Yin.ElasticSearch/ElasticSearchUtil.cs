using Nest;
using System.Linq.Expressions;

namespace Yin.ElasticSearch
{
    public static class ElasticSearchUtil
    {
        /// <summary>
        /// 返回一个正序排列的委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<SortDescriptor<T>, SortDescriptor<T>> Sort<T>(string field) where T : class
        {
            return sd => sd.Ascending(field);
        }

        public static Func<SortDescriptor<T>, SortDescriptor<T>> Sort<T>(Expression<Func<T, object>> field) where T : class
        {
            return sd => sd.Ascending(field);
        }

        /// <summary>
        /// 返回一个倒序排列的委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Func<SortDescriptor<T>, SortDescriptor<T>> SortDesc<T>(string field) where T : class
        {
            return sd => sd.Descending(field);
        }

        public static Func<SortDescriptor<T>, SortDescriptor<T>> SortDesc<T>(Expression<Func<T, object>> field) where T : class
        {
            return sd => sd.Descending(field);
        }

        /// <summary>
        /// 返回一个查询条件集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Func<QueryContainerDescriptor<T>, QueryContainer>> Queries<T>() where T : class
        {
            return new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
        }

        /// <summary>
        /// 添加Match子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要查询的关键字</param>
        /// <param name="boost"></param>
        public static void AddMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field,
            string value, double? boost = null) where T : class
        {
            musts.Add(d => d.Match(mq => mq.Field(field).Query(value).Boost(boost)));
        }

        public static void AddMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, string value) where T : class
        {
            musts.Add(d => d.Match(mq => mq.Field(field).Query(value)));
        }

        /// <summary>
        /// 添加MultiMatch子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="fields">要查询的列</param>
        /// <param name="value">要查询的关键字</param>
        public static void AddMultiMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            string[] fields, string value) where T : class
        {
            musts.Add(d => d.MultiMatch(mq => mq.Fields(fields).Query(value)));
        }

        /// <summary>
        /// 添加MultiMatch子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="fields">例如：f=>new [] {f.xxx, f.xxx}</param>
        /// <param name="value">要查询的关键字</param>
        public static void AddMultiMatch<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> fields, string value) where T : class
        {
            musts.Add(d => d.MultiMatch(mq => mq.Fields(fields).Query(value)));
        }

        /// <summary>
        /// 添加大于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddGreaterThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field,
            double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThan(value)));
        }

        public static void AddGreaterThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThan(value)));
        }

        /// <summary>
        /// 添加大于等于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddGreaterThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field,
            double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThanOrEquals(value)));
        }

        public static void AddGreaterThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).GreaterThanOrEquals(value)));
        }

        /// <summary>
        /// 添加小于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddLessThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field,
            double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThan(value)));
        }

        public static void AddLessThan<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThan(value)));
        }

        /// <summary>
        /// 添加小于等于子句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddLessThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field,
            double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThanOrEquals(value)));
        }

        public static void AddLessThanEqual<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, double value) where T : class
        {
            musts.Add(d => d.Range(mq => mq.Field(field).LessThanOrEquals(value)));
        }

        /// <summary>
        /// 添加日期范围
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public static void AddDateRange<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, DateTime startDate, DateTime endDate) where T : class
        {
            musts.Add(d => d.DateRange(mq => mq.Field(field).GreaterThanOrEquals(startDate).LessThanOrEquals(endDate)));
        }

        /// <summary>
        /// 添加一个Term，一个列一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field">要查询的列</param>
        /// <param name="value">要比较的值</param>
        public static void AddTerm<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field,
            object value) where T : class
        {
            musts.Add(d => d.Term(field, value));
        }

        public static void AddTerm<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, object value) where T : class
        {
            musts.Add(d => d.Term(field, value));
        }

        /// <summary>
        /// 添加一个Terms，一个列多个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="musts"></param>
        /// <param name="field"></param>
        /// <param name="values"></param>
        public static void AddTerms<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts, string field,
            IEnumerable<string> values) where T : class
        {
            musts.Add(d => d.Terms(tq => tq.Field(field).Terms(values)));
        }

        public static void AddTerms<T>(this List<Func<QueryContainerDescriptor<T>, QueryContainer>> musts,
            Expression<Func<T, object>> field, IEnumerable<string> values) where T : class
        {
            musts.Add(d => d.Terms(tq => tq.Field(field).Terms(values)));
        }
    }
}
