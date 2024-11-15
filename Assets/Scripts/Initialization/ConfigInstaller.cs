using Scripts.Features.Grid;
using Scripts.Features.Grid.Matching;
using Scripts.Features.Grid.Moving;
using Scripts.Features.Piece;
using Scripts.Features.Tweening;
using UnityEngine;
using Zenject;

namespace Initialization
{
    public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
    {
        [SerializeField] private GridConfig _gridConfig;
        [SerializeField] private PieceConfig _pieceConfig;
        [SerializeField] private RulesConfig _rulesConfig;
        [SerializeField] private TweenConfig _tweenConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_gridConfig).AsSingle();
            Container.BindInstance(_pieceConfig).AsSingle();
            Container.BindInstance(_rulesConfig).AsSingle();
            Container.BindInstance(_tweenConfig).AsSingle();
        }
    }
}