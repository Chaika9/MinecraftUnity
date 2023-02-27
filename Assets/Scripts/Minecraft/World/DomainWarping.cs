using UnityEngine;

namespace Minecraft.World {
    public class DomainWarping : MonoBehaviour {
        [SerializeField] private NoiseSettings noiseDomainX;
        [SerializeField] private NoiseSettings noiseDomainY;
        [SerializeField] private Vector2Int amplitude;

        public float GenerateDomainNoise(float x, float z, NoiseSettings noiseSettings, int seed) {
            Vector2 domainOffset = GenerateDomainOffset(x, z, seed);
            return NoiseHelper.OctavePerlin(x + domainOffset.x, z + domainOffset.y, noiseSettings, seed);
        }

        private Vector2 GenerateDomainOffset(float x, float z, int seed) {
            float noiseX = NoiseHelper.OctavePerlin(x, z, noiseDomainX, seed) * amplitude.x;
            float noiseY = NoiseHelper.OctavePerlin(x, z, noiseDomainY, seed) * amplitude.y;
            return new Vector2(noiseX, noiseY);
        }
    }
}
