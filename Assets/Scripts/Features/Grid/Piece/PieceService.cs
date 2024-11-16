using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Input;
using Scripts.Features.Spawning;
using Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Scripts.Features.Piece
{
    public class PieceService
    {
        [Inject] private PieceConfig _pieceConfig;
        [Inject] private RulesConfig _rulesConfig;
        [Inject] private GridConfig _gridConfig;
        
        [Inject] private GridService _gridService;
        [Inject] private MatchingService _matchingService;
        [Inject] private MoveService _moveService;
        
        [Inject] private EntityViewPool<PieceEntityView> _pieceEntityViewPool;
        
        [Inject] private EcsWorld _world;
        [Inject] private GridView _gridView;
        
        public int CreateRandomPieceEntity(Vector2Int spawnCoordinates, int height, bool forbidMatches = false)
        {
            var tileEntity = _gridService.GetTileEntity(spawnCoordinates);
            if (_world.GetPool<PieceTileLinkComponent>().Has(tileEntity))
            {
                var coordinates = _world.GetPool<TileComponent>().Get(tileEntity).Coordinates;
                Debug.LogError($"Tile [{coordinates.x}, {coordinates.y}] already has a piece. I've said my piece");
                return ECSTypes.NULL;
            }
            
            var pieceEntity = _world.NewEntity();

            var pieceTypeIndex = GetTypeIndex(forbidMatches, tileEntity);
            
            _world.GetPool<PieceTypeComponent>().Add(pieceEntity) = new PieceTypeComponent
            {
                TypeIndex = pieceTypeIndex,
            };
            
            _world.GetPool<PieceComponent>().Add(pieceEntity) = new PieceComponent();
            LinkTileAndPiece(tileEntity, pieceEntity);
            
            var pieceView = CreatePieceView(pieceEntity, height, tileEntity);
            
            _world.GetPool<PieceViewLinkComponent>().Add(pieceEntity) = new PieceViewLinkComponent
            {
                View = pieceView,
            };
            
            _world.GetPool<PoolableViewComponent>().Add(pieceEntity) = new PoolableViewComponent()
            {
                PoolableEntityView = pieceView,
            };
            
            _world.GetPool<ViewComponent>().Add(pieceEntity) = new ViewComponent()
            {
                View = pieceView,
            };
            
            _moveService.SetupMovePieceCommand(pieceEntity, tileEntity, MoveType.Fall);
            
            return pieceEntity;
        }

        private PieceEntityView CreatePieceView(int pieceEntity, int spawnHeightOffset, int tileEntity)
        {
            var view = _pieceEntityViewPool.GetPooledOrNewView(pieceEntity, _gridView.PiecesParent);
            var offset = new Vector2(0, _gridConfig.TileSize.y * spawnHeightOffset);
            view.RectTransform.anchoredPosition = _gridService.GetTileAnchorPosition(tileEntity) + offset;

            return view;
        }

        private void LinkTileAndPiece(int tileEntity, int pieceEntity)
        {
            _world.GetPool<PieceTileLinkComponent>().Add(pieceEntity) = new PieceTileLinkComponent
            {
                LinkedEntity = tileEntity,
            };

            _world.GetPool<PieceTileLinkComponent>().Add(tileEntity) = new PieceTileLinkComponent
            {
                LinkedEntity = pieceEntity,
            };
        }

        private int GetTypeIndex(bool forbidMatches, int tileEntity)
        {
            var randomIndex = RandomUtils.GetRandomInt(_pieceConfig.PieceTypesCount);
            if (!forbidMatches)
            {
                return randomIndex;
            }
            
            var tileComponent = _world.GetPool<TileComponent>().Get(tileEntity);

            var tileComponentCoordinates = tileComponent.Coordinates;
            //key is the coordinates of the tile, value is the piece type index
            var simulatedTileType = new Dictionary<Vector2Int, int> { {tileComponentCoordinates, 0} };
            for (var i = 0; i < _pieceConfig.PieceTypesCount; i++)
            {
                var randomOffsetIndex = (randomIndex + i) % _pieceConfig.PieceTypesCount;
                simulatedTileType[tileComponentCoordinates] = randomOffsetIndex;
                var allPotentialNeighboursOfSameType = _matchingService.GetMatchingCandidates(tileComponentCoordinates, simulatedTileType);

                var potentialMatches = _matchingService.FindMatchesCoordinates(allPotentialNeighboursOfSameType);
                if (potentialMatches.Count == 0)
                {
                    return randomOffsetIndex;
                }
            }
            
            //TODO Figure out what to do - e.g. refresh some or all of the board
            Debug.LogError("No piece type found that doesn't create a match");
            return randomIndex;
        }
    }
}