using Scripts.Features.Grid;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Initialization
{
    public class GameSceneInstaller : MonoInstaller
    {
        [FormerlySerializedAs("_boardView")] [SerializeField] private GridView _gridView;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gridView).AsSingle();
            
            Container.Bind<GameInitializer>().AsSingle().NonLazy();
            Container.Bind<GridService>().AsSingle();
        }
    }
}
