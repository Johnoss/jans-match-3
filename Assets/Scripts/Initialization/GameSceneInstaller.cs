using Scripts.Features.Grid;
using UnityEngine;
using Zenject;

namespace Initialization
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform _pooledObjectsParent;
        [SerializeField] private GridView _gridView;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gridView).AsSingle();
            
            Container.Bind<GameInitializer>()
                .AsSingle()
                .WithArguments(_pooledObjectsParent)
                .NonLazy();
            Container.Bind<GridService>().AsSingle();
        }
    }
}
