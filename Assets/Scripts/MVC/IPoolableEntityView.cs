namespace MVC
{
    public interface IPoolableEntityView
    {
        void SetEntity(int entity);
        void ResetView();
        void DisableView();
    }
}