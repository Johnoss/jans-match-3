using System.Collections.Generic;
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
        [Inject] private EcsWorld _world;
        [Inject] private GridConfig _gridConfig;
        
        [Inject] private TileView.ViewFactory _tileViewFactory;
        
        [Inject] private ViewPool<PieceView> _pieceViewPool;

        private int[,] _tileEntities;
        public void SetupGrid()
        {
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
                var viewLinkComponent = _world.GetPool<EntityModelLinkComponent>().Get(tileEntity);
                _pieceViewPool.GetPooledOrNewView(-1, viewLinkComponent.ViewGameObject.transform);
            }
        }

        private void CreateTiles()
        {
            for (var x = 0; x < _gridConfig.GridResolution.x; x++)
            {
                for (var y = 0; y < _gridConfig.GridResolution.y; y++)
                {
                    var tileEntity = CreateTileEntity(new Vector2Int(x, y));
                    _tileEntities[x, y] = tileEntity;
                    
                    var tileView = _tileViewFactory.Create(tileEntity);
                    
                    CreateEntityViewLink(tileView, tileEntity);
                }
            }
            SetupNeighbours();
        }

        private void CreateEntityViewLink(TileView tileView, int tileEntity)
        {
            _world.GetPool<EntityModelLinkComponent>().Add(tileEntity) = new EntityModelLinkComponent()
            {
                ViewGameObject = tileView.gameObject,
            };
        }

        private void SetupNeighbours()
        {
            for (var x = 0; x < _gridConfig.GridResolution.x; x++)
            {
                for (var y = 0; y < _gridConfig.GridResolution.y; y++)
                {
                    SetupNeighbour(x, y);
                }
            }
        }

        private void SetupNeighbour(int x, int y)
        {
            var tileEntity = _tileEntities[x, y];
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
    }
}