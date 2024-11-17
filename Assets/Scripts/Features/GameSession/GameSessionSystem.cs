using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using Scripts.Features.Time;

namespace Scripts.Features.GameSession
{
    public class GameSessionSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<GameSessionComponent, GameOverCommand>> _gameOverFilter;
        private EcsFilterInject<Inc<GameTimerComponent, PauseExpireComponent>> _pausedTimerFilter;
        
        private  EcsFilterInject<Inc<PieceComponent>> _pieceFilter;
        private  EcsFilterInject<Inc<SpawnPieceCommand>> _spawnPieceFilter;
        
        private EcsPoolInject<DestroyEntityCommand> _destroyEntityCommandPool;
        
        private EcsCustomInject<GameSessionModel> _gameSessionModel;
        
        public void Run(EcsSystems systems)
        {
            var isBoardBusy = _pausedTimerFilter.Value.GetEntitiesCount() > 0;
            _gameSessionModel.Value.SetTimerPaused(isBoardBusy);
            if (!isBoardBusy)
            {
                _gameSessionModel.Value.ResetCombo();
            }
            
            TryCleanupAfterGameOver();
        }

        private void TryCleanupAfterGameOver()
        {
            if (_gameOverFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var gameSessionEntity in _gameOverFilter.Value)
            {
                _gameOverFilter.Pools.Inc2.Del(gameSessionEntity);
            }
        }
    }
}