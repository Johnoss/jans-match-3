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

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        
        public abstract void ResetView();
        public abstract void DisableView();
    }
}