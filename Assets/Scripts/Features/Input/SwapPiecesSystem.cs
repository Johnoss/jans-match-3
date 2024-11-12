using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;

namespace Scripts.Features.Input
{
    public class SwapPiecesSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SwapPieceCommand, PieceTileLinkComponent, PieceViewLinkComponent>> _swapPieceCommands;
        
        private readonly EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private readonly EcsPoolInject<MoveToTileComponent> _moveToTilePool;

        private readonly EcsCustomInject<GridService> _gridService; 
            
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _swapPieceCommands.Value)
            {
                var swapPieceCommandComponent = _swapPieceCommands.Pools.Inc1.Get(pieceEntity);
                var targetTileEntity = swapPieceCommandComponent.TargetEntity;
                
                _gridService.Value.SetTilePieceLink(targetTileEntity, pieceEntity);
                
                _swapPieceCommands.Pools.Inc1.Del(pieceEntity);
                
                _moveToTilePool.Value.Add(pieceEntity) = new MoveToTileComponent();
            }
        }
    }
}