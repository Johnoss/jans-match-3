using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Time;

namespace Scripts.Utils
{
    public static class EcsUtils
    {
        public static ref T GetOrAddComponent<T>(this EcsPool<T> pool, int entity) where T : struct
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
        
        public static void AddOrSkip<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (!pool.Has(entity))
            {
                pool.Add(entity);
            }
        }
        
        public static EcsSystems Add(this EcsSystems systems, params IEcsSystem[] systemsToAdd) 
        {
            foreach (var system in systemsToAdd)
            {
                systems.Add(system);
            }
            return systems;
        }
        
        public static void UpdateTime<TComponent>(this EcsPool<TComponent> pool, int entity) where TComponent : struct, ITimeComponent
        {
            ref var component = ref pool.Get(entity);
            component.RemainingSeconds -= UnityEngine.Time.deltaTime;
            
            if (component.RemainingSeconds <= 0)
            {
                pool.Del(entity);
            }
        }

        public static bool IsInAnyPool(this int entity, params IEcsPool[] pools)
        {
            if (entity == ECSTypes.NULL)
            {
                return false;
            }
            
            return pools.Length != 0 && pools.Any(pool => pool.Has(entity));
        }
        
        public static bool HasAnyOf(this IEcsPool pool, params int[] entities)
        {
            return entities.Length != 0 && entities.Any(pool.Has);
        }
        
        public static bool HasAllOf(this IEcsPool pool, params int[] entities)
        {
            return entities.Length == 0 || entities.All(pool.Has);
        }
    }
}