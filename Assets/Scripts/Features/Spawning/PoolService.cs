using MVC;
using Zenject;

namespace Scripts.Features.Spawning
{
    public class PoolService
    {
        [Inject] private DiContainer _container;
        
        public EntityViewPool<T> GetPool<T>() where T : PoolableEntityView
        {
            //TODO refactor
            return !_container.HasBinding<EntityViewPool<T>>() ? null : _container.Resolve<EntityViewPool<T>>();
        }
        
        public bool TryAddToPool<T>(T view) where T : PoolableEntityView
        {
            var pool = GetPool<T>();
            if (pool == null)
            {
                return false;
            }
            pool.AddView(view);
            return true;
        }
    }
}