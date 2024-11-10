using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Piece;
using UnityEngine;

namespace Scripts.Features.Input
{
    public class InputSystem : IEcsRunSystem
    {
        private const int MAX_INTERACTED_TILES = 2;
        
        //TODO add IsBusy or IsReady component
        private readonly EcsFilterInject<Inc<UserInteractingComponent, TileViewLinkComponent>, Exc<BlockInteractionComponent>> _isInteractingFilter;
        
        private readonly EcsPoolInject<UserInteractingComponent> _userInteractingPool;
        private readonly EcsPoolInject<SwapPieceCommand> _swapPieceCommandPool;
        private readonly EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        
        private readonly EcsCustomInject<GridService> _gridService;

        private readonly List<int> _interactedTileEntities = new();
        
        public void Run(EcsSystems systems)
        {
            if(_isInteractingFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }
            
            _interactedTileEntities.Clear();
            
            if(!UnityEngine.Input.GetMouseButton(0) && UnityEngine.Input.touches.Length == 0)
            {
                ClearInteraction();
                return;
            }

            if (_isInteractingFilter.Value.GetEntitiesCount() < MAX_INTERACTED_TILES)
            {
                return;
            }
            
            foreach (var tileEntity in _isInteractingFilter.Value)
            {
                _interactedTileEntities.Add(tileEntity);

                if (_interactedTileEntities.Count < MAX_INTERACTED_TILES)
                {
                    continue;
                }
                
                TrySwapTiles();
                return;
            }
        }

        private void ClearInteraction()
        {
            foreach (var entity in _isInteractingFilter.Value)
            {
                _userInteractingPool.Value.Del(entity);
            }
            
            Debug.Log("Clearing interaction");
        }
        
        private void TrySwapTiles()
        {
            var originTileEntity = _interactedTileEntities[0];
            var targetTileEntity = _interactedTileEntities[1];
            
            if (!_gridService.Value.AreNeighbours(originTileEntity, targetTileEntity))
            {
                //TODO shake effect
                ClearInteraction();
                return;
            }
            
            var originPieceEntity = _pieceTileLinkPool.Value.Get(originTileEntity).LinkedEntity;
            var targetPieceEntity = _pieceTileLinkPool.Value.Get(targetTileEntity).LinkedEntity;
            
            _swapPieceCommandPool.Value.Add(originPieceEntity) = new SwapPieceCommand
            {
                TargetEntity = targetTileEntity,
            };
            
            _swapPieceCommandPool.Value.Add(targetPieceEntity) = new SwapPieceCommand
            {
                TargetEntity = originTileEntity,
            };
            
            Debug.Log($"Swapping tiles {originTileEntity} and {targetTileEntity}");
            ClearInteraction();
        }
    }
}