using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;

namespace Scripts.Features.Input
{
    public class SwapPiecesSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SwapPieceComponent, PieceTileLinkComponent, PieceViewLinkComponent>, Exc<MoveToTileComponent>> _swapPieceFilter;
        
        private readonly EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private readonly EcsPoolInject<MoveToTileComponent> _moveToTilePool;
        
        private readonly EcsCustomInject<GridService> _gridService; 
            
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _swapPieceFilter.Value)
            {
                var swapPieceCommandComponent = _swapPieceFilter.Pools.Inc1.Get(pieceEntity);
                var targetTileEntity = swapPieceCommandComponent.TargetTileEntity;
                
                _gridService.Value.SetTilePieceLink(targetTileEntity, pieceEntity);
                
                _moveToTilePool.Value.Add(pieceEntity) = new MoveToTileComponent();
            }
        }
    }
}