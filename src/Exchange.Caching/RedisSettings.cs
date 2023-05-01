using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Caching;
public class RedisSettings
{
    public string RedisDataProtectionKey { get; set; }
    public int CacheTime { get; set; }
    public string RedisConnectionString { get; set; }
    public int? RedisDatabaseId { get; set; }
}
