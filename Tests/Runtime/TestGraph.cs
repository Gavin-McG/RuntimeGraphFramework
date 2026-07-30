using UnityEngine;

namespace RuntimeGraphFramework.Tests
{
    public class TestGraph : RuntimeGraph
    {
        private static bool HasRegistered = false;

        [RuntimeInitializeOnLoadMethod]
        public static void RegisterTypeCasts()
        {
            if (HasRegistered) return;

            PortTypeCastManager.Register<float, int, TestGraph>(FloatToInt);
            PortTypeCastManager.Register<float, string, TestGraph>(FloatToString);
            
            PortTypeCastManager.Register<int, float, TestGraph>(IntToFloat);
            PortTypeCastManager.Register<int, string, TestGraph>(IntToString);
            
            HasRegistered = true;
        }
        
        private static int FloatToInt(float value) => (int)value;
        private static string FloatToString(float value) => value.ToString();
        
        private static float IntToFloat(int value) => value;
        private static string IntToString(int value) => value.ToString();
    }
}
