using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid;
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
        
        [Inject] private GridService _gridService;
        [Inject] private EcsWorld _world;
        
        [Inject] private EntityViewPool<PieceEntityView> _pieceEntityViewPool;
        
        public int CreateRandomPieceEntity(int tileEntity)
        {
            var pieceEntity = _world.NewEntity();

            var pieceTypeIndex = RandomUtils.GetRandomInt(_pieceConfig.PieceTypesCount);
            
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
    }
}