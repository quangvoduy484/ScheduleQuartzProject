using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduleQuartz.Controllers
{
    [ApiController]
    [Route("api/cache")]
    public  class CacheController : ControllerBase
    {
        private  readonly IMemoryCache _cache;
        private static CancellationTokenSource _clearToken = new CancellationTokenSource();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixes = new ConcurrentDictionary<string, CancellationTokenSource>();
        public CacheController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpPost("send-cache")]
        public async Task<IActionResult> AddOrUpdateCached()
        {
            //set expiration time for the passed cache key
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));

            _cache.GetOrCreate("quang", entry =>
            {
                 entry.SetOptions(options);
                 return "deptrai";
            });

            //set expiration time for the passed cache key
            var options1 = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            options1.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));

            _cache.GetOrCreate("quang1", entry =>
            {
                entry.SetOptions(options1);
                return "deptrai1";
            });

            return Ok();
        }

   
        [HttpPost("remove-prefixe")]
        public async Task<IActionResult> RemoveByPrefix()
        {
            string prefix = "quangnam";
            _prefixes.TryRemove(prefix, out var tokenSource);
            tokenSource?.Cancel();
            tokenSource?.Dispose();

            return Ok();
        }

        [HttpPost("cancel-cache")]
        public async Task<IActionResult> CancelToken()
        {
            var value = _cache.Get("quang");
            var value1 = _cache.Get("quang1");

            _clearToken.Cancel();
            _clearToken.Dispose();

            _clearToken = new CancellationTokenSource();

            return Ok();
        }
    }
}
