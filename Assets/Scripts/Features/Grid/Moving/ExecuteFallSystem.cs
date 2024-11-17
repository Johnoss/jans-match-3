using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;

namespace Scripts.Features.Grid.Moving
{
    public class ExecuteFallSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<FallPieceCommand, PieceComponent>> _fallingPiecesFilter;
        
        private EcsPoolInject<StartMovePieceCommand> _startMovePieceCommandPool;
        
        private EcsCustomInject<GridService> _gridService;
        private EcsCustomInject<MoveService> _moveService;
        
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _fallingPiecesFilter.Value)
            {
                var isFallingComponent = _fallingPiecesFilter.Pools.Inc1.Get(pieceEntity);
                
                var targetCoordinates = isFallingComponent.FallCoordinates;
                var targetTile = _gridService.Value.GetTileEntity(targetCoordinates);
                
                _moveService.Value.SetupMovePieceCommand(pieceEntity, targetTile, MoveType.Fall);
            }
        }
    }
}