using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Grid.Matching
{
    public class ShuffleBoardSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NoPossibleMovesComponent>> _noPossibleMovesFilter;
        
        private EcsFilterInject<Inc<PieceComponent, PieceTileLinkComponent>> _piecesFilter;
        
        private EcsPoolInject<MoveToTileComponent> _moveToTilePool;
        
        private EcsCustomInject<GridService> _gridService;
        
        public void Run(EcsSystems systems)
        {
            if (_noPossibleMovesFilter.Value.GetEntitiesCount() <= 0)
            {
                return;
            }
            
            foreach (var entity in _noPossibleMovesFilter.Value)
            {
                _noPossibleMovesFilter.Pools.Inc1.Del(entity);
            }
            
            ShuffleBoard();
        }

        private void ShuffleBoard()
        {
            var shuffledCoordinates = _gridService.Value.GetShuffledTileCoordinates();
            var shuffledCoordinatesQueue = new Queue<Vector2Int>(shuffledCoordinates);
            foreach (var pieceEntity in _piecesFilter.Value)
            {
                var targetCoordinates = shuffledCoordinatesQueue.Dequeue();
                
                _gridService.Value.SetTilePieceLink(targetCoordinates, pieceEntity);
                
                _moveToTilePool.Value.Add(pieceEntity) = new MoveToTileComponent();
            }
        }
    }
}