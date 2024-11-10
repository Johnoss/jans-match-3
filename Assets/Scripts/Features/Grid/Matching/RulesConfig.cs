using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Features.Grid.Matching
{
    [CreateAssetMenu(menuName = "Create RulesConfig", fileName = "RulesConfig", order = 0)]
    public class RulesConfig : ScriptableObject
    {
        [Header("Rules")] [SerializeField] private int _minPatternLength = 3;

        public int MinPatternLength => _minPatternLength;

        [Header("Patterns")] [SerializeField] private PatternSetting[] _legalPatternSettings = new[]
        {
            new PatternSetting { Pattern = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) } },
            new PatternSetting { Pattern = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) } },
            new PatternSetting { Pattern = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0) } },
            new PatternSetting { Pattern = new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) } },
            new PatternSetting { Pattern = new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3) } },
            new PatternSetting { Pattern = new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4) } },
            new PatternSetting { Pattern = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) } }
        };

        public PatternSetting[] LegalPatternSettings => _legalPatternSettings;
    }
    
    [System.Serializable]
    public class PatternSetting
    {
        public Vector2Int[] Pattern;
        //TODO implement powerups
        //public BoosterSetting BoosterSetting;
    }
}
