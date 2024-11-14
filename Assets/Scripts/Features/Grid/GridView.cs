using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Features.Grid
{
    public class GridView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private RectTransform _piecesParent;

        [Inject] private GridConfig _gridConfig;

        public Transform TilesParent => _gridLayoutGroup.transform;
        public Transform PiecesParent => _piecesParent;

        public void SetupBoard()
        {
            _gridLayoutGroup.constraintCount = _gridConfig.GridResolution.x;
            _gridLayoutGroup.cellSize = _gridConfig.TileSize;
        }
    }
}