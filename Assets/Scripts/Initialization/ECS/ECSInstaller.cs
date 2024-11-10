using Leopotam.EcsLite;
using Zenject;

namespace Initialization.ECS
{
    public class ECSInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EcsWorld>().AsSingle();
            Container.Bind<ECSIntializer>().AsSingle().NonLazy();
        }
    }
}