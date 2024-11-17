using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Grid.Moving
{
    public class SetupFallSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<TileComponent, FallOccupantComponent, PieceTileLinkComponent>> _occupiedFallingTiles;
        private EcsFilterInject<Inc<FallOccupantComponent>> _emptyFallingTiles;

        private EcsCustomInject<GridService> _gridService;
        private EcsPoolInject<FallPieceCommand> _fallPiecePool;
        
        public void Run(EcsSystems systems)
        {
            SetupPiecesToFall();
        }

        private void SetupPiecesToFall()
        {
            foreach (var tileEntity in _occupiedFallingTiles.Value)
            {
                var tileComponent = _occupiedFallingTiles.Pools.Inc1.Get(tileEntity);
                var tileLinkComponent = _occupiedFallingTiles.Pools.Inc3.Get(tileEntity);
                var pieceEntity = tileLinkComponent.LinkedEntity;
                
                if (_fallPiecePool.Value.Has(pieceEntity))
                {
                    continue;
                }
                
                var emptyTiles = _gridService.Value.GetEmptyTilesBelow(tileComponent.Coordinates); 
                var fallDistance = emptyTiles.Count;
                var fallCoordinates = tileComponent.Coordinates + Vector2Int.down * fallDistance;
                
                _fallPiecePool.Value.Add(pieceEntity) = new FallPieceCommand
                {
                    FallCoordinates = fallCoordinates,
                };
            }
        }
    }
}