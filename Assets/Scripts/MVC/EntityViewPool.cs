using System.Collections.Generic;
using System.Linq;
using Initialization.ECS;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace MVC
{
    [UsedImplicitly]
    public class EntityViewPool<TView>
        where TView : PoolableEntityView
    {
        private readonly Queue<TView> _views = new();

        private IFactory<int, TView> _viewFactory;
        private Transform _pooledObjectsParent;

        public void SetupPool(int initialViewCount, IFactory<int, TView> viewFactory, Transform pooledObjectsParent)
        {
            _viewFactory = viewFactory;
            _pooledObjectsParent = pooledObjectsParent;
            
            for (var i = 0; i < initialViewCount; i++)
            {
                AddView(_viewFactory.Create(ECSTypes.NULL));
            }
        }

        public TView GetPooledOrNewView(int entity, Transform parent)
        {
            var view = _views.Any() ? _views.Dequeue() : _viewFactory.Create(entity);
            view.SetParent(parent);
            view.SetEntity(entity);
            view.ResetView();
            return view;
        }

        public void AddView(TView view)
        {
            view.SetParent(_pooledObjectsParent);
            view.SetEntity(ECSTypes.NULL);
            view.DisableView();
            _views.Enqueue(view);
            
            //TODO dispose subscriptions
        }
    }
}