using Unity.Mathematics;

namespace Scripts.Utils
{
    public static class RandomUtils
    {
        private static Random _random;

        static RandomUtils()
        {
            _random = new Random();
            _random.InitState();
        }
        
        public static int GetRandomInt(int max)
        {
            return GetRandomInt(0, max);
        }

        public static int GetRandomInt(int min, int max)
        {
            return _random.NextInt(min, max);
        }
    }
}