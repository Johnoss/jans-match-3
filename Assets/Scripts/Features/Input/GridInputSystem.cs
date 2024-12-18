using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Input
{
    public class GridInputSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<GridInputComponent>, Exc<BlockContinuousInputComponent>> _gridInputFilter;
        private EcsFilterInject<Inc<BlockContinuousInputComponent>> _blockInputFilter;
        private EcsFilterInject<Inc<SelectedPieceComponent>> _selectedPieceFilter;
        private EcsFilterInject<Inc<SwapPieceComponent>> _swapPieceFilter;
        private EcsFilterInject<Inc<IsMovingComponent>> _isMovingFilter;
        private EcsFilterInject<Inc<CollectPieceComponent>> _collectPieceFilter;
        
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private EcsPoolInject<SelectedPieceComponent> _selectedPiecePool;
        
        private EcsCustomInject<InputService> _inputService;
        
        private readonly EcsCustomInject<GridService> _gridService;

        public void Run(EcsSystems systems)
        {
            if (_swapPieceFilter.Value.GetEntitiesCount() + _isMovingFilter.Value.GetEntitiesCount() + _collectPieceFilter.Value.GetEntitiesCount() > 0)
            {
                return;
            }
            
            DetermineGridInput();
            TryUnblockInput();
        }

        private void TryUnblockInput()
        {
            var isTap = _selectedPieceFilter.Value.GetEntitiesCount() == 1;
            if (UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.touchCount != 0 || isTap)
            {
                return;
            }
            
            //mouse/finger released, we can interact again (prevents continuous swapping while dragging)
            _inputService.Value.ToggleContinuousInteractionBlock(false);
            ClearInteraction();
        }

        private void DetermineGridInput()
        {
            foreach (var entity in _gridInputFilter.Value)
            {
                if (!TryGetPointerPosition(out var pointerPosition))
                {
                    return;
                }

                if (!_inputService.Value.IsPointerOverTile(pointerPosition, out var tileCoordinates))
                {
                    return;
                }
                
                var tileEntity = _gridService.Value.GetTileEntity(tileCoordinates);
                MarkTilePieceForInteraction(tileEntity);
            }
        }

        private static bool TryGetPointerPosition(out Vector3 pointerPosition)
        {
            pointerPosition = Vector3.zero;
            if (UnityEngine.Input.GetMouseButton(0))
            {
                pointerPosition = UnityEngine.Input.mousePosition;
            }
            else if(UnityEngine.Input.touches.Length > 0)
            {
                pointerPosition = UnityEngine.Input.touches[0].position;
            }
            else
            {
                return false;
            }

            return true;
        }

        private void MarkTilePieceForInteraction(int tileEntity)
        {
            if(!_pieceTileLinkPool.Value.Has(tileEntity))
            {
                return;
            }
            
            var pieceEntity = _pieceTileLinkPool.Value.Get(tileEntity).LinkedEntity;
            _selectedPiecePool.Value.AddOrSkip(pieceEntity);
        }

        private void ClearInteraction()
        {
            foreach (var entity in _selectedPieceFilter.Value)
            {
                _selectedPieceFilter.Pools.Inc1.Del(entity);
            }
        }
    }
}