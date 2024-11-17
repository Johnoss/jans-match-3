using UnityEngine;

namespace Scripts.Utils
{
    //TODO serialize data into json. This is a quick and dirty solution
    public static class PlayerPrefsUtils
    {
        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        
        public static float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
    }
}