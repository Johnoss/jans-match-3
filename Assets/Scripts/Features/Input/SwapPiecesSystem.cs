using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Input
{
    public class SwapPiecesSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SwapPieceCommand, PieceTileLinkComponent, PieceViewLinkComponent>> _swapPieceCommands;
        
        private readonly EcsPoolInject<TileViewLinkComponent> _tileViewLinkPool;
        private readonly EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _swapPieceCommands.Value)
            {
                var swapPieceCommandComponent = _swapPieceCommands.Pools.Inc1.Get(pieceEntity);
                ref var linkToTileComponent = ref _swapPieceCommands.Pools.Inc2.Get(pieceEntity);
                var pieceViewLinkComponent = _swapPieceCommands.Pools.Inc3.Get(pieceEntity);

                var targetTileEntity = swapPieceCommandComponent.TargetEntity;
                linkToTileComponent.LinkedEntity = targetTileEntity;
                
                _swapPieceCommands.Pools.Inc1.Del(pieceEntity);
                
                if (!_tileViewLinkPool.Value.Has(targetTileEntity))
                {
                    Debug.LogError($"TileViewLinkComponent not found for entity {targetTileEntity}");
                    continue;
                }
                
                //TODOtween
                var tileViewLinkComponent = _tileViewLinkPool.Value.Get(targetTileEntity);
                ref var linkToPieceComponent = ref _pieceTileLinkPool.Value.Get(targetTileEntity);
                linkToPieceComponent.LinkedEntity = pieceEntity;
                var pieceView = pieceViewLinkComponent.View;
                pieceView.transform.SetParent(tileViewLinkComponent.View.PieceAnchor);
                pieceView.RectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}