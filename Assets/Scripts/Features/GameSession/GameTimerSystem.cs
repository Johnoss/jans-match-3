using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Input;
using Scripts.Features.Piece;
using Scripts.Features.Time;
using Scripts.Utils;

namespace Scripts.Features.GameSession
{
    public class GameTimerSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<GameTimerComponent, GameInProgressComponent, ExpireComponent>> _timerFilter;
        private EcsFilterInject<Inc<GameTimerComponent, GameInProgressComponent>, Exc<ExpireComponent>> _timeOverFilter;
        
        private EcsCustomInject<GameSessionModel> _gameSessionModel;
        
        private  EcsFilterInject<Inc<SwapPieceComponent>> _swapPieceFilter;
        private  EcsFilterInject<Inc<PieceComponent, IsMovingComponent>> _isMovingFilter;
        private  EcsFilterInject<Inc<PieceComponent, StartMovePieceCommand>> _startMoveFilter;
        private  EcsFilterInject<Inc<CollectPieceComponent>> _collectPieceFilter;
        private  EcsFilterInject<Inc<PieceComponent, IsTweeningComponent>> _tweeningPiecesFilter;

        private EcsPoolInject<PauseExpireComponent> _pauseExpirePool;
            
        public void Run(EcsSystems systems)
        {
            var isMatchOngoing = IsMatchOngoing();
            _gameSessionModel.Value.SetTimerPaused(isMatchOngoing);
            
            foreach (var entity in _timerFilter.Value)
            {
                if (isMatchOngoing)
                {
                    _pauseExpirePool.Value.AddOrSkip(entity);
                }
                else
                {
                    _pauseExpirePool.Value.DelOrSkip(entity);
                }
                
                var expireComponent = _timerFilter.Pools.Inc3.Get(entity);
                _gameSessionModel.Value.SetRemainingSeconds(expireComponent.RemainingSeconds);
                
            }

            foreach (var entity in _timeOverFilter.Value)
            {
                _gameSessionModel.Value.SetRemainingSeconds(0);
                
                _timeOverFilter.Pools.Inc2.Del(entity);
            }
        }

        private bool IsMatchOngoing()
        {
            return _swapPieceFilter.Value.GetEntitiesCount() 
                + _isMovingFilter.Value.GetEntitiesCount()
                + _startMoveFilter.Value.GetEntitiesCount()
                + _collectPieceFilter.Value.GetEntitiesCount()
                + _tweeningPiecesFilter.Value.GetEntitiesCount() > 0;
        }
    }
}