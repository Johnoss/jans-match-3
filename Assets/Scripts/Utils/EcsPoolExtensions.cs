using Leopotam.EcsLite;

namespace Scripts.Utils
{
    public static class EcsPoolExtensions
    {
        public static ref T GetOrGetComponent<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            return ref pool.Has(entity) ? ref pool.Get(entity) : ref pool.Add(entity);
        }
        
        public static void DeleteComponent<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (pool.Has(entity))
            {
                pool.Del(entity);
            }
        }
    }
}