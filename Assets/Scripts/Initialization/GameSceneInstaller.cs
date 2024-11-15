using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Input;
using Scripts.Features.Piece;
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

            Container.Bind<PieceService>().AsSingle();
            Container.Bind<GridService>().AsSingle();
            Container.Bind<MatchingService>().AsSingle();
            Container.Bind<MoveService>().AsSingle();
            Container.Bind<InputService>().AsSingle();
        }
    }
}
