using Scripts.Features.Grid;
using Scripts.Features.Piece;
using UnityEngine;
using Zenject;

namespace Initialization
{
    public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
    {
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private PieceConfig _pieceConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gridConfig).AsSingle();
            Container.BindInstance(_pieceConfig).AsSingle();
        }
    }
}