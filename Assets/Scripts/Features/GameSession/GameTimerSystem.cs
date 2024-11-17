using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Time;

namespace Scripts.Features.GameSession
{
    public class GameTimerSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<GameTimerComponent, GameInProgressComponent, ExpireComponent>> _timerFilter;
        private EcsFilterInject<Inc<GameTimerComponent, GameInProgressComponent>, Exc<ExpireComponent>> _timeOverFilter;
        
        private EcsCustomInject<GameSessionModel> _gameSessionModel;
            
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _timerFilter.Value)
            {
                var expireComponent = _timerFilter.Pools.Inc3.Get(entity);
                _gameSessionModel.Value.SetRemainingSeconds(expireComponent.RemainingSeconds);
            }

            foreach (var entity in _timeOverFilter.Value)
            {
                _gameSessionModel.Value.SetRemainingSeconds(0);
                
                _timeOverFilter.Pools.Inc2.Del(entity);
            }
        }
    }
}