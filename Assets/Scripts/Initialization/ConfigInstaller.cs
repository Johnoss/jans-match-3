using Scripts.Features.Grid;
using UnityEngine;
using Zenject;

namespace Initialization
{
    public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
    {
        [SerializeField] private GridConfig _gridConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gridConfig).AsSingle();
        }
    }
}