using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Scripts.Features.Time
{
    public class ExpireSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ExpireComponent>> _expireFilter;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _expireFilter.Value)
            {
                ref var expireComponent = ref _expireFilter.Pools.Inc1.Get(entity);
                expireComponent.RemainingSeconds -= UnityEngine.Time.deltaTime;
                
                if (expireComponent.RemainingSeconds <= 0)
                {
                    _expireFilter.Pools.Inc1.Del(entity);
                }
            }
        }
    }
}