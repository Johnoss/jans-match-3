using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;

namespace Scripts.Features.GameSession
{
    public class GameSessionSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<GameSessionComponent, GameOverCommand>> _gameOverFilter;
        
        private  EcsFilterInject<Inc<PieceComponent>> _pieceFilter;
        private  EcsFilterInject<Inc<SpawnPieceCommand>> _spawnPieceFilter;
        
        private EcsPoolInject<DestroyEntityCommand> _destroyEntityCommandPool;
        
        public void Run(EcsSystems systems)
        {
            if (_gameOverFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }

            foreach (var pieceEntity in _pieceFilter.Value)
            {
                _destroyEntityCommandPool.Value.Add(pieceEntity);
            }
            
            foreach (var gameSessionEntity in _spawnPieceFilter.Value)
            {
                _destroyEntityCommandPool.Value.Add(gameSessionEntity);
            }

            foreach (var gameSessionEntity in _gameOverFilter.Value)
            {
                _gameOverFilter.Pools.Inc2.Del(gameSessionEntity);
                
            }
        }
    }
}