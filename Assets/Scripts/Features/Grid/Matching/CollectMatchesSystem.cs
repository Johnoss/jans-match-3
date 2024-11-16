using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using Scripts.Features.Time;
using Scripts.Features.Tweening;

namespace Scripts.Features.Grid.Matching
{
    public class CollectMatchesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PieceComponent, IsMatchComponent, PieceTileLinkComponent>, Exc<IsTweeningComponent>> _isMatchFilter;

        private EcsPoolInject<DestroyEntityCommand> _destroyEntityCommandPool;
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private EcsPoolInject<SpawnPieceCommand> _spawnPieceCommandPool; 

        private EcsCustomInject<GridService> _gridService;
        
        private EcsCustomInject<EcsWorld> _world;
        private EcsCustomInject<TweenConfig> _tweenConfig;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _isMatchFilter.Value)
            {
                _destroyEntityCommandPool.Value.Add(pieceEntity) = new DestroyEntityCommand();

                SetupSpawnNewPiece(pieceEntity);

                //TODO score and celebration
            }
        }

        private void SetupSpawnNewPiece(int pieceEntity)
        {
            var spawnEntity = _world.Value.NewEntity();
            _spawnPieceCommandPool.Value.Add(spawnEntity) = new SpawnPieceCommand
            {
                ForbidMatches = false,
                Column = _gridService.Value.GetPieceCoordinates(pieceEntity).x
            };
        }
    }
}