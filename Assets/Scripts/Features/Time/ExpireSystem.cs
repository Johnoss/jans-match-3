using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Moving;
using Scripts.Utils;

namespace Scripts.Features.Time
{
    public class ExpireSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ExpireComponent>> _expireFilter;
        private EcsFilterInject<Inc<IsTweeningComponent>> _cooldownFilter;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _expireFilter.Value)
            {
                _expireFilter.Pools.Inc1.UpdateTime(entity);
            }

            foreach (var entity in _cooldownFilter.Value)
            {
                _cooldownFilter.Pools.Inc1.UpdateTime(entity);
            }
        }
    }
}