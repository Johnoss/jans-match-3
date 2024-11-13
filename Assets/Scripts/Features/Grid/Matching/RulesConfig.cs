using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Features.Grid.Matching
{
    public class RulesConfig : ScriptableObject
    {
        [Header("Matches")]
        [SerializeField] private int _minMatchLength = 3;
        [SerializeField] private bool _allowSquareMatches = true;
        public int MinMatchLength => _minMatchLength;
        public bool AllowSquareMatches => _allowSquareMatches;
        
        [Header("Possible Moves")]
        [SerializeField] private int _maxIterationsPerFrame = 10;
        
        public int MaxIterationsPerFrame => _maxIterationsPerFrame;
    }
}
