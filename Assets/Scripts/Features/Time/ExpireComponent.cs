using Scripts.Features.Grid.Moving;

namespace Scripts.Features.Time
{
    public struct ExpireComponent : ITimeComponent
    {
        public float RemainingSeconds { get; set; }
    }
}
