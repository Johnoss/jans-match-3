using Leopotam.EcsLite;
using UniRx;
using Zenject;

namespace Initialization.ECS
{
    public class ECSIntializer
    {
        [Inject] private EcsWorld _world;
        
        private EcsSystems _systems;
        
        [Inject]
        private void Init()
        {
            _systems = new EcsSystems(_world);

            _systems
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Init();
            
            Observable.EveryUpdate().Subscribe(_ => RunSystems());
        }

        private void RunSystems()
        {
            _systems.Run();
        }
    }
}