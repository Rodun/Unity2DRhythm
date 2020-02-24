#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("fITahMjMgRxp/QW/X0Zm3c7gqMg9Z7VaCbns+T6IUTX7fHqBooNL5SHU72a5uicYrMnROf3BXhz/s+ll4Jg3rIaqQWqBTFNK+oki4y9AaWNd3tDf713e1d1d3t7fXE/0ukcAtZ0yy8zLAO4mWzWz0lITT5LOs6sM+IzZ0Yl00eO8wIy7obC3RIvwiUatb286MB8W24ynmCUZKgy3XmHgCeSladn0LB9Wr1QDuEI70rWpBEt53e8q1XqHDk9OmDY6CDE5s4W44Q4BxKPJy9uCC47xQJruXS5UU18gVO51Gj27M42cAw6vbZ7ElVamNgGZ713e/e/S2db1WZdZKNLe3t7a39wKjh+g2rbxNE42KJzLXRySlnAV94eq/ic9bE3CON3c3t/e");
        private static int[] order = new int[] { 4,10,9,12,10,7,8,7,13,13,13,12,13,13,14 };
        private static int key = 223;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
