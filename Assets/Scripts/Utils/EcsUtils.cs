using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using Scripts.Features.Time;
using UnityEngine;

namespace Scripts.Utils
{
    public static class EcsUtils
    {
        public static ref T GetOrAddComponent<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            return ref pool.Has(entity) ? ref pool.Get(entity) : ref pool.Add(entity);
        }
        
        public static void DelOrSkip<T>(this EcsPool<T> pool, int entity, out bool hasDeleted) where T : struct
        {
            var has = pool.Has(entity);
            hasDeleted = !has;
            if (has)
            {
                pool.Del(entity);
            }
        }
        
        public static void DelOrSkip<T>(this EcsPool<T> pool, int entity) where T : struct
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
        
        public static void AddOrSkip<T>(this EcsPool<T> pool, int entity, out bool hasAdded) where T : struct
        {
            var has = pool.Has(entity);
            hasAdded = !has;
            if (!has)
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
            Debug.Log("Frame: " + Time.frameCount);
            Debug.Log($"Updating time for entity {entity}, previous value: {component.RemainingSeconds}");
            var deltaTime = Time.deltaTime;
            component.RemainingSeconds -= deltaTime;
            Debug.Log($"Updated time for entity {entity}, new value: {component.RemainingSeconds}, deltaTime: {deltaTime}");
            
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