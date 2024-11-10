using System.Linq;
using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Piece;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Grid
{
    public class GridService
    {
        [Inject] private GridConfig _gridConfig;
        
        [Inject] private PieceService _pieceService;
        
        [Inject] private EcsWorld _world;
        [Inject] private GridView _gridView;
        
        [Inject] private TileView.ViewFactory _tileViewFactory;

        private int[,] _tileEntities;
        public void SetupGrid()
        {
            _gridView.SetupBoard();
            var gridEntity = _world.NewEntity();
            _world.GetPool<GridComponent>().Add(gridEntity) = new GridComponent();
            
            _tileEntities = new int[_gridConfig.GridResolution.x, _gridConfig.GridResolution.y];
            
            CreateTiles();
            PopulateTiles();
        }

        private void PopulateTiles()
        {
            foreach (var tileEntity in _tileEntities)
            {
                _pieceService.CreateRandomPieceEntity(tileEntity);
            }
        }

        private void CreateTiles()
        {
            for (var row = 0; row < _gridConfig.GridResolution.y; row++)
            {
                for (var column = 0; column < _gridConfig.GridResolution.x; column++)
                {
                    var tileEntity = CreateTileEntity(new Vector2Int(column, row));
                    _tileEntities[column, row] = tileEntity;
                    
                    var tileView = _tileViewFactory.Create(tileEntity);
                    
                    CreateEntityViewLink(tileView, tileEntity);
                }
            }
            SetupNeighbours();
        }

        private void CreateEntityViewLink(TileView tileView, int tileEntity)
        {
            _world.GetPool<TileViewLinkComponent>().Add(tileEntity) = new TileViewLinkComponent()
            {
                View = tileView,
            };
        }

        private void SetupNeighbours()
        {
            for (var column = 0; column < _gridConfig.GridResolution.x; column++)
            {
                for (var row = 0; row < _gridConfig.GridResolution.y; row++)
                {
                    SetupNeighbour(column, row);
                }
            }
        }

        private void SetupNeighbour(int column, int row)
        {
            var tileEntity = _tileEntities[column, row];
            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);
            var neighboringTileEntities = tileComponent.NeighboringTileCoordinates
                .Select(neighborCoordinate => _tileEntities[neighborCoordinate.x, neighborCoordinate.y]).ToArray();
                    
            _world.GetPool<NeighboursComponent>().Add(tileEntity) = new NeighboursComponent()
            {
                NeighboringTileEntities = neighboringTileEntities,
            };
        }

        private int CreateTileEntity(Vector2Int coordinates)
        {
            var entity = _world.NewEntity();
            
            //Check bounds and add valid neighbours
            var validNeighbours = _gridConfig.NeighboringOffsets
                .Select(neighboringOffset => coordinates + neighboringOffset)
                .Where(neighbor => neighbor.IsWithinBounds(_gridConfig.GridResolution)).ToArray();

            _world.GetPool<TileComponent>().Add(entity) = new TileComponent
            {
                Coordinates = coordinates,
                NeighboringTileCoordinates = validNeighbours
            };
            
            return entity;
        }

        public bool AreNeighbours(int interactedTileA, int interactedTileB)
        {
            var tileA = _world.GetPool<TileComponent>().Get(interactedTileA);
            var tileB = _world.GetPool<TileComponent>().Get(interactedTileB);

            return tileA.NeighboringTileCoordinates.Contains(tileB.Coordinates);
        }
    }
}