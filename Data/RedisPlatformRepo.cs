using System.Text.Json;
using RedisAPI.Models;
using StackExchange.Redis;

namespace RedisAPI.Data;

public class RedisPlatformRepo : IPlatformRepo
{
    private readonly IConnectionMultiplexer _redis;
    
    public RedisPlatformRepo(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform == null)
        {
            throw new ArgumentOutOfRangeException(nameof(platform));
        }

        var db = _redis.GetDatabase();
        var serialPlat = JsonSerializer.Serialize(platform);

        // db.StringSet(platform.Id, serialPlat);
        // db.SetAdd("PlatformSet", serialPlat);

        db.HashSet("hashPlatform", [
            new HashEntry(platform.Id, serialPlat)
        ]);
    }

    public Platform? GetPlatformById(string id)
    {
        var db = _redis.GetDatabase();

        // var plat = db.StringGet(id);
        var plat = db.HashGet("hashplatform", id);

        if (!string.IsNullOrEmpty(plat))
        {
            return JsonSerializer.Deserialize<Platform>(plat);
        }

        return null;
    }

    public IEnumerable<Platform?>? GetAllPlatforms()
    {
        var db = _redis.GetDatabase();

        // var completeSet = db.SetMembers("PlatformSet");

        var completeHash = db.HashGetAll("hashplatform");

        if (completeHash.Length > 0)
        {
            var obj = Array.ConvertAll(completeHash, val => JsonSerializer.Deserialize<Platform>(val.Value)).ToList();

            return obj;
        }
        
        return null;
    }
}