using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;

namespace Scripts.Features.Input
{
    public class InputSystem : IEcsRunSystem
    {
        private const int MAX_INTERACTED_TILES = 2;
        
        private readonly EcsFilterInject<Inc<UserInteractingComponent, TileViewLinkComponent>, Exc<IsMovingComponent, SpawnTargetComponent>> _isInteractingFilter;
        
        private EcsFilterInject<Inc<SwapPieceComponent>> _swapPieceFilter;
        
        private readonly EcsPoolInject<UserInteractingComponent> _userInteractingPool;
        private readonly EcsPoolInject<SwapPieceComponent> _swapPieceCommandPool;
        private readonly EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        
        private readonly EcsCustomInject<GridService> _gridService;

        private readonly List<int> _interactedTileEntities = new();
        
        public void Run(EcsSystems systems)
        {
            if(_isInteractingFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }
            
            if(_swapPieceFilter.Value.GetEntitiesCount() > 0)
            {
                //already swapping
                ClearInteraction();
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
        }
        
        private void TrySwapTiles()
        {
            var originTileEntity = _interactedTileEntities[0];
            var targetTileEntity = _interactedTileEntities[1];

            if (!_gridService.Value.AreNeighbours(originTileEntity, targetTileEntity) ||
                !_pieceTileLinkPool.Value.Has(originTileEntity) ||
                !_pieceTileLinkPool.Value.Has(targetTileEntity))
            {
                ClearInteraction();
                return;
            }

            var originPieceEntity = _pieceTileLinkPool.Value.Get(originTileEntity).LinkedEntity;
            var targetPieceEntity = _pieceTileLinkPool.Value.Get(targetTileEntity).LinkedEntity;
            
            if(_swapPieceCommandPool.Value.Has(originPieceEntity) || _swapPieceCommandPool.Value.Has(targetPieceEntity))
            {
                ClearInteraction();
                return;
            }
            
            _swapPieceCommandPool.Value.Add(originPieceEntity) = new SwapPieceComponent
            {
                SourceTileEntity = originTileEntity,
                TargetTileEntity = targetTileEntity,
                TargetPieceEntity = targetPieceEntity,
            };
            
            _swapPieceCommandPool.Value.Add(targetPieceEntity) = new SwapPieceComponent
            {
                SourceTileEntity = targetTileEntity,
                TargetTileEntity = originTileEntity,
                TargetPieceEntity = originPieceEntity,
            };
            
            ClearInteraction();
        }
    }
}