using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Features.Grid.Matching
{
    [CreateAssetMenu(menuName = "Create RulesConfig", fileName = "RulesConfig", order = 0)]
    public class RulesConfig : ScriptableObject
    {
        [Header("Matches")]
        [SerializeField] private int _minMatchLength = 3;
        [SerializeField] private bool _allowSquareMatches = true;

        public int MinMatchLength => _minMatchLength;
        public bool AllowSquareMatches => _allowSquareMatches;
    }
}
