using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Piece;

namespace Scripts.Features.Spawning
{
    public class SpawnPieceSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SpawnTargetComponent, TileComponent>> _spawnTargetsFilter;
        
        private EcsCustomInject<PieceService> _pieceService;
        
        private EcsCustomInject<MatchingService> _matchingService;
        
        public void Run(EcsSystems systems)
        {
            foreach (var spawnTileEntity in _spawnTargetsFilter.Value)
            {
                var spawnComponent = _spawnTargetsFilter.Pools.Inc1.Get(spawnTileEntity);
                _pieceService.Value.CreateRandomPieceEntity(spawnTileEntity, spawnComponent.ForbidMatches);
                
                _spawnTargetsFilter.Pools.Inc1.Del(spawnTileEntity);
                
                _matchingService.Value.OnBoardUpdated();
            }
        }
    }
}