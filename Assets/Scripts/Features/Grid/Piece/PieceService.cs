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
        
        [Inject] private GridService _gridService;
        [Inject] private MatchingService _matchingService;
        [Inject] private EcsWorld _world;
        
        [Inject] private EntityViewPool<PieceEntityView> _pieceEntityViewPool;
        
        public int CreateRandomPieceEntity(int tileEntity, bool forbidMatches = false)
        {
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
            
            var pieceView = CreatePieceView(pieceEntity, tileEntity);
            
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
            
            _world.GetPool<ChangedPositionComponent>().Add(pieceEntity) = new ChangedPositionComponent();
            
            return pieceEntity;
        }

        private PieceEntityView CreatePieceView(int pieceEntity, int tileEntity)
        {
            var tileView = _world.GetPool<TileViewLinkComponent>().Get(tileEntity).View;
            var view = _pieceEntityViewPool.GetPooledOrNewView(pieceEntity, tileView.PieceAnchor);

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
            for (var i = 0; i < _pieceConfig.PieceTypesCount; i++)
            {
                var randomOffsetIndex = (randomIndex + i) % _pieceConfig.PieceTypesCount;
                var allPotentialNeighboursOfSameType = _matchingService.GetMatchingCandidates(tileComponentCoordinates, randomOffsetIndex);

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