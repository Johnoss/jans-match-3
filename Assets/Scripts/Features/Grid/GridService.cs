using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
using Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Grid
{
    public class GridService
    {
        [Inject] private GridConfig _gridConfig;
        
        [Inject] private PieceService _pieceService;
        [Inject] private MatchingService _matchingService;
        
        [Inject] private EcsWorld _world;
        [Inject] private GridView _gridView;
        
        [Inject] private TileView.ViewFactory _tileViewFactory;

        private int[,] _tileEntities;
        
        public void SetupGrid()
        {
            _gridView.SetupGridView();
            var gridEntity = _world.NewEntity();
            _world.GetPool<GridComponent>().Add(gridEntity) = new GridComponent();
            
            _tileEntities = new int[_gridConfig.GridResolution.x, _gridConfig.GridResolution.y];
            
            CreateTiles();
        }

        public bool AreNeighbours(int interactedTileA, int interactedTileB)
        {
            var tileA = _world.GetPool<TileComponent>().Get(interactedTileA);
            var tileB = _world.GetPool<TileComponent>().Get(interactedTileB);

            return tileA.NeighboringTileCoordinates.Contains(tileB.Coordinates);
        }
        
        public HashSet<int> GetPieceEntitiesAtCoordinates(HashSet<Vector2Int> coordinates)
        {
            return coordinates
                .Select(GetPieceEntity)
                .Where(pieceEntity => pieceEntity != ECSTypes.NULL).ToHashSet();
        }

        public int GetTileEntity(Vector2Int coordinates)
        {
            return _tileEntities[coordinates.x, coordinates.y];
        }

        public int GetPieceEntity(int column, int row)
        {
            var tileEntity = _tileEntities[column, row];
            return !_world.GetPool<PieceTileLinkComponent>().Has(tileEntity)
                ? ECSTypes.NULL
                : _world.GetPool<PieceTileLinkComponent>().Get(tileEntity).LinkedEntity;
        }
        
        public int GetPieceEntity(Vector2Int coordinates)
        {
            return GetPieceEntity(coordinates.x, coordinates.y);
        }

        public HashSet<int> GetEmptyTilesBelow(Vector2Int tileComponentCoordinates)
        {
            var emptyTiles = new HashSet<int>();
            for (var row = tileComponentCoordinates.y - 1; row >= 0; row--)
            {
                var tileEntity = _tileEntities[tileComponentCoordinates.x, row];
                if (!_world.GetPool<PieceTileLinkComponent>().Has(tileEntity))
                {
                    emptyTiles.Add(tileEntity);
                }
            }

            return emptyTiles;
        }

        public HashSet<int> GetTilesAbove(Vector2Int tileComponentCoordinates)
        {
            var tilesAbove = new HashSet<int>();
            for (var row = tileComponentCoordinates.y + 1; row < _gridConfig.GridResolution.y; row++)
            {
                tilesAbove.Add(_tileEntities[tileComponentCoordinates.x, row]);
            }

            return tilesAbove;
        }
        
        public bool TryGetNextCoordinates(Vector2Int currentCoordinates, out Vector2Int nextCoordinates)
        {
            var totalColumns = _gridConfig.GridResolution.x;
            var totalRows = _gridConfig.GridResolution.y;

            var nextX = (currentCoordinates.x + 1) % totalColumns;
            var nextY = currentCoordinates.y + (currentCoordinates.x + 1) / totalColumns;

            nextCoordinates = new Vector2Int(nextX, nextY);
            return nextY < totalRows;
        }

        public void SetTilePieceLink(Vector2Int targetTileCoordinates, int pieceEntity)
        {
            var targetTileEntity = GetTileEntity(targetTileCoordinates);
            SetTilePieceLink(targetTileEntity, pieceEntity);
        }
        
        public void SetTilePieceLink(int targetTileEntity, int pieceEntity)
        {
            var pieceTileLinkPool = _world.GetPool<PieceTileLinkComponent>();

            ref var pieceToTileLink = ref pieceTileLinkPool.GetOrAddComponent(pieceEntity);
            ref var tileToPieceLink = ref pieceTileLinkPool.GetOrAddComponent(targetTileEntity);
            
            pieceToTileLink.LinkedEntity = targetTileEntity;
            tileToPieceLink.LinkedEntity = pieceEntity;
        }

        public void UnlinkPieceFromTile(int pieceEntity)
        {
            var pieceTileLinkPool = _world.GetPool<PieceTileLinkComponent>();

            if (!pieceTileLinkPool.Has(pieceEntity))
            {
                return;
            }
            
            var currentTileEntity = pieceTileLinkPool.Get(pieceEntity).LinkedEntity;
            pieceTileLinkPool.DelOrSkip(currentTileEntity);
        }
        
        public IEnumerable<Vector2Int> GetShuffledTileCoordinates()
        {
            return _gridConfig.GridResolution.GetShuffledCoordinates();
        }

        public Vector2 GetTileAnchorPosition(int tileEntity)
        {
            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);
            return _gridConfig.GetTileAnchorPosition(tileComponent.Coordinates);
        }

        private void CreateTiles()
        {
            for (var row = 0; row < _gridConfig.GridResolution.y; row++)
            {
                for (var column = 0; column < _gridConfig.GridResolution.x; column++)
                {
                    var coordinates = new Vector2Int(column, row);
                    var tileEntity = CreateTileEntity(coordinates);
                    _tileEntities[column, row] = tileEntity;
                    
                    CreateTileView(tileEntity, coordinates);
                }
            }
            SetupNeighbours();
        }

        private void CreateTileView(int tileEntity, Vector2Int coordinates)
        {
            var tileView = _tileViewFactory.Create(tileEntity);
            _world.GetPool<TileViewLinkComponent>().Add(tileEntity) = new TileViewLinkComponent
            {
                View = tileView,
            };
            
            _world.GetPool<ViewComponent>().Add(tileEntity) = new ViewComponent
            {
                View = tileView,
            };
            
            tileView.RectTransform.anchoredPosition = _gridConfig.GetTileAnchorPosition(coordinates);
            tileView.RectTransform.sizeDelta = _gridConfig.TileSize;
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
                    
            _world.GetPool<NeighboursComponent>().Add(tileEntity) = new NeighboursComponent
            {
                NeighboringTileEntities = neighboringTileEntities,
            };
        }

        private int CreateTileEntity(Vector2Int coordinates)
        {
            var tileEntity = _world.NewEntity();

            //Check bounds and add valid neighbours
            var validNeighbours = _gridConfig.NeighboringOffsets
                .Select(neighboringOffset => coordinates + neighboringOffset)
                .Where(neighbor => neighbor.IsWithinBounds(_gridConfig.GridResolution)).ToArray();

            _world.GetPool<TileComponent>().Add(tileEntity) = new TileComponent
            {
                Coordinates = coordinates,
                NeighboringTileCoordinates = validNeighbours
            };
            
            _world.GetPool<SpawnTargetComponent>().Add(tileEntity) = new SpawnTargetComponent
            {
                ForbidMatches = true,
            };

            return tileEntity;
        }
    }
}