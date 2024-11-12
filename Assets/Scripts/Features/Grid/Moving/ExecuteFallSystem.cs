using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Grid.Moving
{
    public class ExecuteFallSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<IsFallingComponent, PieceTileLinkComponent, PieceComponent>> _fallingPiecesFilter;
        
        private EcsPoolInject<MoveToTileComponent> _moveToTilePool;
        
        private EcsCustomInject<GridService> _gridService;
        
        public void Run(EcsSystems systems)
        {
            foreach (var pieceEntity in _fallingPiecesFilter.Value)
            {
                var isFallingComponent = _fallingPiecesFilter.Pools.Inc1.Get(pieceEntity);
                
                _gridService.Value.UnlinkPieceFromTile(pieceEntity);
                
                var targetCoordinates = _gridService.Value.GetPieceCoordinates(pieceEntity) + Vector2Int.down * isFallingComponent.FallDistance;
                var targetTile = _gridService.Value.GetTileEntity(targetCoordinates);
                
                _gridService.Value.SetTilePieceLink(targetTile, pieceEntity);
                
                _moveToTilePool.Value.Add(pieceEntity) = new MoveToTileComponent();
                
                _fallingPiecesFilter.Pools.Inc1.Del(pieceEntity);
                
            }
        }
    }
}