using MVC;
using Scripts.Features.Grid;
using Scripts.Features.Piece;
using UnityEngine;
using Zenject;

namespace Initialization
{
    public class FactoryInstaller : MonoInstaller
    {   
        [Inject] private GridConfig _gridConfig;
        [Inject] private PieceConfig _pieceConfig;
        
        [Inject] private GridView _gridView;
        
        public override void InstallBindings()
        {
            Container.Bind<ViewPool<PieceView>>().AsSingle();
            
            Container.BindFactory<int, PieceView, PieceView.ViewFactory>()
                .FromComponentInNewPrefab(_pieceConfig.PiecePrefab);
            
            Container.BindFactory<int, TileView, TileView.ViewFactory>()
                .FromComponentInNewPrefab(_gridConfig.TilePrefab)
                .UnderTransform(_gridView.TilesParent);
        }
    }
}