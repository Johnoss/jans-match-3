using UnityEngine;

namespace MVC
{
    public abstract class PoolableEntityView : AbstractView
    {
        protected int Entity;

        public abstract void ReturnToPool();
        
        public void SetEntity(int entity)
        {
            Entity = entity;
        }

        public void SetParent(Transform parent, bool worldPositionStays = false)
        {
            transform.SetParent(parent, worldPositionStays);
        }
        
        public abstract void ResetView();
        public abstract void DisableView();
    }
}