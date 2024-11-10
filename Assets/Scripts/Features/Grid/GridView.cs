using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Features.Grid
{
    public class GridView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        
        private GridConfig _gridConfig;

        public Transform TilesParent => _gridLayoutGroup.transform;
    }
}