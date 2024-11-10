using Leopotam.EcsLite;
using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Input;
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
            
            _world.GetPool<PieceTypeComponent>().Add(pieceEntity) = new PieceTypeComponent()
            {
                TypeIndex = pieceTypeIndex,
            };
            
            _world.GetPool<PieceComponent>().Add(pieceEntity) = new PieceComponent();
            LinkTileAndPiece(tileEntity, pieceEntity);

            
            var pieceView = CreatePieceView(pieceEntity, pieceTypeIndex, tileEntity);
            
            _world.GetPool<PieceViewLinkComponent>().Add(pieceEntity) = new PieceViewLinkComponent()
            {
                View = pieceView,
            };
            
            
            return pieceEntity;
        }

        private PieceEntityView CreatePieceView(int pieceEntity, int pieceTypeIndex, int tileEntity)
        {
            var tileView = _world.GetPool<TileViewLinkComponent>().Get(tileEntity).View;
            var view = _pieceEntityViewPool.GetPooledOrNewView(pieceEntity, tileView.PieceAnchor);

            return view;
        }

        private void LinkTileAndPiece(int tileEntity, int pieceEntity)
        {
            _world.GetPool<PieceTileLinkComponent>().Add(pieceEntity) = new PieceTileLinkComponent()
            {
                LinkedEntity = tileEntity,
            };

            _world.GetPool<PieceTileLinkComponent>().Add(tileEntity) = new PieceTileLinkComponent()
            {
                LinkedEntity = pieceEntity,
            };
        }
    }
    
    public struct PieceTypeComponent
    {
        public int TypeIndex;
    }
}