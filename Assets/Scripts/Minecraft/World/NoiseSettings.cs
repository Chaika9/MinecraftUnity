using UnityEngine;

namespace Minecraft.World {
    [CreateAssetMenu(fileName = "NoiseSettings", menuName = "Minecraft/Noise Settings")]
    public class NoiseSettings : ScriptableObject {
        public float scale;
        public int octaves;
        public float persistence;
        public Vector2Int offset;
        public float redistribution;
        public float exponent;
    }
}
