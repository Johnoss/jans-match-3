namespace Scripts.Features.Piece
{
    public struct PieceTileLinkComponent
    {
        public int LinkedEntity;
        public static PieceTileLinkComponent Null => new() { LinkedEntity = -1 };
    }
}