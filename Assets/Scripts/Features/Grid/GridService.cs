using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Features.Spawning;
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
            _gridView.SetupBoard();
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

        public HashSet<int> GetPiecesToFall(Vector2Int tileComponentCoordinates)
        {
            var column = tileComponentCoordinates.x;
            var startingRow = tileComponentCoordinates.y;
            var piecesAbove = new HashSet<int>();
            for (var row = startingRow + 1; row < _gridConfig.GridResolution.y; row++)
            {
                var pieceEntity = GetPieceEntity(column, row);
                if (pieceEntity == ECSTypes.NULL || _world.GetPool<IsFallingComponent>().Has(pieceEntity))
                {
                    continue;
                }
                
                piecesAbove.Add(pieceEntity);
            }

            return piecesAbove;
        }
        
        public int GetFallDistance(HashSet<int> piecesToFall, Vector2Int emptyTileCoordinates)
        {
            var bottomPieceY = piecesToFall
                .Select(GetPieceCoordinates)
                .Min(pieceCoordinates => pieceCoordinates.y);
            
            return bottomPieceY - emptyTileCoordinates.y;
        }

        public Vector2Int GetPieceCoordinates(int pieceEntity)
        {
            if (!_world.GetPool<PieceTileLinkComponent>().Has(pieceEntity))
            {
                Debug.LogError("Trying to get coordinates of a piece that is not linked to a tile");
                return Vector2Int.zero;
            }
            
            var pieceTile = _world.GetPool<PieceTileLinkComponent>().Get(pieceEntity).LinkedEntity;
            return _world.GetPool<TileComponent>().Get(pieceTile).Coordinates;
        }

        public void SetTilePieceLink(int targetTileEntity, int pieceEntity)
        {
            //TODO GetOrCreate helper method
            var pieceTileLinkPool = _world.GetPool<PieceTileLinkComponent>();
            
            //UnlinkPieceFromTile(pieceEntity);

            ref var pieceToTileLink = ref !pieceTileLinkPool.Has(pieceEntity)
                ? ref pieceTileLinkPool.Add(pieceEntity)
                : ref pieceTileLinkPool.Get(pieceEntity);
                
            ref var tileToPieceLink = ref !pieceTileLinkPool.Has(targetTileEntity)
                ? ref pieceTileLinkPool.Add(targetTileEntity)
                : ref pieceTileLinkPool.Get(targetTileEntity);
            
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
            if (pieceTileLinkPool.Has(currentTileEntity))
            {
                pieceTileLinkPool.Del(currentTileEntity);
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
                    
                    _world.GetPool<ViewComponent>().Add(tileEntity) = new ViewComponent
                    {
                        View = tileView,
                    };
                }
            }
            SetupNeighbours();
        }

        private void CreateEntityViewLink(TileView tileView, int tileEntity)
        {
            _world.GetPool<TileViewLinkComponent>().Add(tileEntity) = new TileViewLinkComponent
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