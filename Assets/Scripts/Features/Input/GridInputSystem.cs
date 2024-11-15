using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Scripts.Features.Grid;
using Scripts.Features.Piece;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Features.Input
{
    public class GridInputSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<GridInputComponent>, Exc<BlockInputComponent>> _gridInputFilter;
        private EcsFilterInject<Inc<BlockInputComponent>> _blockInputFilter;
        
        private EcsPoolInject<PieceTileLinkComponent> _pieceTileLinkPool;
        private EcsPoolInject<SelectedPieceComponent> _selectedPiecePool;
        
        private EcsCustomInject<InputService> _inputService;
        
        private readonly EcsCustomInject<GridService> _gridService;

        public void Run(EcsSystems systems)
        {
            DetermineGridInput();
            
            TryUnblockInput();
        }

        private void TryUnblockInput()
        {
            foreach (var entity in _blockInputFilter.Value)
            {
                if (UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.touchCount != 0)
                {
                    return;
                }
                
                //mouse/finger released, we can interact again (prevents swapping while dragging)
                _blockInputFilter.Pools.Inc1.Del(entity);
            }
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
    }
}