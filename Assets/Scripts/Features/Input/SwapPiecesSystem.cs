using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;

namespace Scripts.Features.Input
{
    public class SwapPiecesSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SwapPieceComponent, PieceTileLinkComponent, PieceViewLinkComponent>, Exc<IsMovingComponent>> _swapPieceFilter;
        
        private readonly EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        
        private readonly EcsCustomInject<GridService> _gridService; 
        private readonly EcsCustomInject<MoveService> _moveService; 
            
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _swapPieceFilter.Value)
            {
                var swapPieceCommandComponent = _swapPieceFilter.Pools.Inc1.Get(pieceEntity);
                var targetTileEntity = swapPieceCommandComponent.TargetTileEntity;
                
                var swapType = swapPieceCommandComponent.IsReverting ? MoveType.RevertSwap : MoveType.Swap;
                
                _moveService.Value.SetupMovePieceCommand(pieceEntity, targetTileEntity, swapType);
            }
        }
    }
}