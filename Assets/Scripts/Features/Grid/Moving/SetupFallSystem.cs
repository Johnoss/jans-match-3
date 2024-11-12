using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using UnityEngine;

namespace Scripts.Features.Grid.Moving
{
    public class SetupFallSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<TileComponent>, Exc<PieceTileLinkComponent, SpawnTargetComponent>> _emptyTilesFilter;

        private EcsCustomInject<GridService> _gridService;
        private EcsPoolInject<IsFallingComponent> _isFallingPool;
        private EcsPoolInject<MoveToTileComponent> _moveToTilePool;
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private EcsPoolInject<SpawnTargetComponent> _spawnTargetPool;
        
        public void Run(EcsSystems systems)
        {
            foreach (var tileEntity in _emptyTilesFilter.Value)
            {
                var tileComponent = _emptyTilesFilter.Pools.Inc1.Get(tileEntity);
                var emptyTileCoordinates = tileComponent.Coordinates;
                var piecesToFall = _gridService.Value.GetPiecesToFall(tileComponent.Coordinates);
                
                if (piecesToFall.Count == 0)
                {
                    _spawnTargetPool.Value.Add(tileEntity) = new SpawnTargetComponent();
                    continue;
                }
                
                MarkPiecesAsFalling(piecesToFall, emptyTileCoordinates);
            }
        }

        private void MarkPiecesAsFalling(HashSet<int> piecesToFall, Vector2Int emptyTileCoordinates)
        {
            var fallDistance = _gridService.Value.GetFallDistance(piecesToFall, emptyTileCoordinates);
            foreach (var pieceEntity in piecesToFall.Where(pieceEntity => !_isFallingPool.Value.Has(pieceEntity)))
            {
                _isFallingPool.Value.Add(pieceEntity) = new IsFallingComponent()
                {
                    FallDistance = fallDistance,
                };
                
                //SetTargetTile(fallDistance, pieceEntity);

                //_moveToTilePool.Value.Add(pieceEntity) = new MoveToTileComponent();
            }
        }

        private void SetTargetTile(int fallDistance, int pieceEntity)
        {
            var pieceCoordinates = _gridService.Value.GetPieceCoordinates(pieceEntity);
            var targetCoordinates = pieceCoordinates + Vector2Int.down * fallDistance;
            var targetTileEntity = _gridService.Value.GetTileEntity(targetCoordinates);

            _gridService.Value.SetTilePieceLink(targetTileEntity, pieceEntity);
        }
    }
}