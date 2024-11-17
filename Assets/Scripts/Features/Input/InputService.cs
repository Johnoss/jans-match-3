using Leopotam.EcsLite;
using Scripts.Features.Grid;
using Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Input
{
    public class InputService
    {
        [Inject] private EcsWorld _world;

        [Inject] private GridConfig _gridConfig;

        [Inject] private GridView _gridView;

        private RectTransform TilesParent => _gridView.TilesParent;

        public int InputEntity { get; private set; }

        private Vector2Int _gridResolution;
        private Vector2 _tileSize;
        private Vector2 _gridOriginOffset;

        public void SetupInput()
        {
            InputEntity = _world.NewEntity();
            _world.GetPool<GridInputComponent>().Add(InputEntity);

            InitializeGridMetrics();
        }

        public bool IsPointerOverTile(Vector3 screenPointerPosition, out Vector2Int tileCoordinates)
        {
            tileCoordinates = Vector2Int.zero;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(TilesParent, screenPointerPosition, null, out var localPointerPosition))
            {
                return false;
            }

            var adjustedPointerPosition = localPointerPosition - _gridOriginOffset;

            var x = Mathf.FloorToInt(adjustedPointerPosition.x / _tileSize.x);
            var y = Mathf.FloorToInt(adjustedPointerPosition.y / _tileSize.y);

            if (x < 0 || x >= _gridResolution.x || y < 0 || y >= _gridResolution.y)
            {
                return false;
            }

            tileCoordinates = new Vector2Int(x, y);
            return true;
        }

        public void ToggleContinuousInteractionBlock(bool isBlocked)
        {
            var blockInteractionPool = _world.GetPool<BlockContinuousInputComponent>();
            if (isBlocked)
            {
                blockInteractionPool.AddOrSkip(InputEntity);
            }
            else
            {
                blockInteractionPool.DelOrSkip(InputEntity);
            }
        }

        private void InitializeGridMetrics()
        {
            _gridResolution = _gridConfig.GridResolution;
            _tileSize = _gridConfig.TileSize;

            _gridOriginOffset = new Vector2(-(_gridResolution.x * _tileSize.x * TilesParent.pivot.x), -(_gridResolution.y * _tileSize.y * TilesParent.pivot.y));
        }
    }
}
