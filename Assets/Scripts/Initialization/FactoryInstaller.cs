using Scripts.Features.Grid;
using Zenject;
using static Scripts.Features.Grid.TileView;

namespace Initialization
{
    public class FactoryInstaller : MonoInstaller
    {
        [Inject] private GridConfig _gridConfig;
        [Inject] private GridView _gridView;
        
        public override void InstallBindings()
        {
            Container.BindFactory<int, TileView, TileFactory>()
                .FromComponentInNewPrefab(_gridConfig.TilePrefab)
                .UnderTransform(_gridView.TilesParent);
        }
    }
}