using System;
using UniRx;
using Zenject;

namespace MVC
{
    public abstract class MVCDisposable : IDisposable
    {
        [Inject] protected readonly CompositeDisposable Disposer;

        public virtual void Dispose()
        {
            Disposer?.Dispose();
        }
    }
}