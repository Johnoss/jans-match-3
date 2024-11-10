using Leopotam.EcsLite;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Grid
{
    public class GridService
    {
        [Inject] private EcsWorld _world;
        [Inject] private GridConfig _gridConfig;
        
        [Inject] private TileView.TileFactory _tileFactory;
        
        public void SetupGrid()
        {
            var gridEntity = _world.NewEntity();
            _world.GetPool<GridComponent>().Add(gridEntity) = new GridComponent();
            
            CreateTiles();
        }

        private void CreateTiles()
        {
            for (var x = 0; x < _gridConfig.GridSize.x; x++)
            {
                for (var y = 0; y < _gridConfig.GridSize.y; y++)
                {
                    var tileEntity = CreateTileEntity(new Vector2Int(x, y));
                    _tileFactory.Create(tileEntity);
                }
            }
        }

        private int CreateTileEntity(Vector2Int coordinates)
        {
            var entity = _world.NewEntity();
            
            _world.GetPool<TileComponent>().Add(entity) = new TileComponent
            {
                Coordinates = coordinates
            };
            
            return entity;
        }
    }

    public struct GridComponent
    {
    }

    public struct TileComponent
    {
        public Vector2Int Coordinates;
    }
}