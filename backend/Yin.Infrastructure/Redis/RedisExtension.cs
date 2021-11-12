using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Yin.Infrastructure.Redis
{
    public static class RedisExtension
    {
        public static async Task<bool> StringSetAsync(this IDatabase db, string key, object value, TimeSpan? expiry = null)
        {
            string str = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return await db.StringSetAsync(key, str, expiry);
        }

        public static async Task<T> StringGetAsync<T>(this IDatabase db, string key)
        {
            var str = await db.StringGetAsync(key);
            if (string.IsNullOrWhiteSpace(str))
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
        }
        public static T StringGet<T>(this IDatabase db, string key)
        {
            var str = db.StringGet(key);
            if (string.IsNullOrWhiteSpace(str))
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
        }
        public static async Task<bool> HashSetAsync(this IDatabase db, string key, string field, object value)
        {
            string str = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return await db.HashSetAsync(key, field, str);
        }
        public static async Task<T> HashGetAsync<T>(this IDatabase db, string key, string field)
        {
            var str = await db.HashGetAsync(key, field);
            if (string.IsNullOrWhiteSpace(str))
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
        }

        /// <summary>
        /// 模糊查询
        /// 使用 scan ， keys 影响性能
        /// http://doc.redisfans.com/key/scan.html
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pattern">参数来匹配</param>
        /// <returns></returns>
        public static async Task<string[]> GetKeysAsync(this IDatabase db, string pattern)
        {
            return (string[])await db.ScriptEvaluateAsync(
                LuaScript.Prepare($"local dbsize=redis.call('dbsize')  local res=redis.call('scan',0,'match','{pattern}','count',dbsize)  return res[2]"));
        }
        /// <summary>
        /// 批量删除key
        /// </summary>
        /// <param name="db"></param>
        /// <param name="pattern">参数匹配</param>
        /// <returns></returns>
        public static async Task<bool> KeyBatchDeleteAsync(this IDatabase db, string pattern)
        {
            var keys = (await db.GetKeysAsync(pattern)).Select(t => (RedisKey)t).ToArray();
            return await db.KeyDeleteAsync(keys) > 0;
        }

        public static HashEntry[] ToHashEntries(this object obj)
        {
            var properties = obj.GetType().GetProperties();
            return properties.Select(t =>
                    new HashEntry(t.Name, t.GetValue(obj).ToString()))
                .ToArray();
        }

        public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
        {
            var properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                var entity = hashEntries.FirstOrDefault(t => t.Name.ToString().Equals(
                    property.Name));
                if (entity.Equals(new HashEntry())) continue;
                if (property.PropertyType.Name.Equals(nameof(Guid)))
                {
                    property.SetValue(obj, Guid.Parse(entity.Value.ToString()));
                }
                else
                {
                    property.SetValue(obj, Convert.ChangeType(entity.Value.ToString(),
                        property.PropertyType));
                }
            }

            return (T)obj;
        }
    }
}
