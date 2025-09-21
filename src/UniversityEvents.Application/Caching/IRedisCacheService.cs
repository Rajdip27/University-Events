using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace UniversityEvents.Application.Caching;

public interface IRedisCacheService
{
    /// <summary>
    /// Asynchronously retrieves data associated with the specified key and deserializes it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the retrieved data will be deserialized.</typeparam>
    /// <param name="key">The unique identifier for the data to retrieve. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized data of type T, or
    /// the default value of T if the key does not exist.</returns>
    Task<T> GetDataAsync<T>(string key, CancellationToken cancellationToken);
    /// <summary>
    /// Asynchronously sets the data associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the data to store.</typeparam>
    /// <param name="key">The key that identifies the data to set. Cannot be null or empty.</param>
    /// <param name="data">The data to associate with the specified key.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous set operation.</returns>
    Task SetDataAsync<T>(string key, T data, CancellationToken cancellationToken);
    /// <summary>
    /// Asynchronously removes the data associated with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier of the data to remove. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    Task RemoveDataAsync(string key, CancellationToken cancellationToken);
}
public class RedisCacheService(IDistributedCache cache) : IRedisCacheService
{
    /// <summary>
    /// Asynchronously retrieves and deserializes data associated with the specified key from the cache.
    /// </summary>
    /// <remarks>If the specified key does not exist in the cache or the cached data is empty, the method
    /// returns the default value for type T. The method uses JSON deserialization; ensure that the cached data is valid
    /// JSON for the specified type.</remarks>
    /// <typeparam name="T">The type to which the cached data is deserialized.</typeparam>
    /// <param name="key">The key that identifies the cached data to retrieve. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object of type T if
    /// the data is found and successfully deserialized; otherwise, the default value for type T.</returns>
    public async Task<T> GetDataAsync<T>(string key, CancellationToken cancellationToken)
    {
        string data = await cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(data))
            return default;

        return JsonSerializer.Deserialize<T>(data);
    }
    /// <summary>
    /// Asynchronously removes the data associated with the specified key from the cache.
    /// </summary>
    /// <param name="key">The unique key identifying the data to remove. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the remove operation.</param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    public async Task RemoveDataAsync(string key, CancellationToken cancellationToken)
            => await cache.RemoveAsync(key, cancellationToken);

    /// <summary>
    /// Asynchronously stores the specified data in the distributed cache under the given key.
    /// </summary>
    /// <remarks>The cached entry will expire five minutes after being set. If an entry with the same key
    /// already exists, it will be overwritten.</remarks>
    /// <typeparam name="T">The type of the data to store in the cache. The data will be serialized to JSON before being cached.</typeparam>
    /// <param name="key">The key under which the data will be stored. Cannot be null or empty.</param>
    /// <param name="data">The data to store in the cache. The data will be serialized to JSON.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous cache set operation.</returns>
    public async Task SetDataAsync<T>(
        string key,
        T data,
        CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(data),
            options);
    }
}