using UnityEngine;

namespace Minecraft.World {
    public static class NoiseHelper {
        public static float OctavePerlin(float x, float z, float scale, int octaves, Vector2Int offset, float persistence, int seed) {
            // Apply scale
            x *= scale;
            z *= scale;
            x += scale;
            z += scale;

            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0; // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < octaves; i++) {
                total += Mathf.PerlinNoise((x + offset.x + seed) * frequency,
                    (z + offset.y + seed) * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= 2;
            }
            return total / maxValue;
        }

        public static float OctavePerlin(float x, float z, NoiseSettings noiseSettings, int seed) {
            return OctavePerlin(x, z, noiseSettings.scale, noiseSettings.octaves, noiseSettings.offset, noiseSettings.persistence, seed);
        }
        
        public static float Redistribute(float value, float redistribution, float exponent) {
            return Mathf.Pow(value * redistribution, exponent);
        }

        public static float Redistribute(float value, NoiseSettings noiseSettings) {
            return Redistribute(value, noiseSettings.redistribution, noiseSettings.exponent);
        }

        public static float RemapValue(float value, float initialMin, float initialMax, float finalMin, float finalMax) {
            return finalMin + (value - initialMin) * (finalMax - finalMin) / (initialMax - initialMin);
        }

        public static float RemapValue(float value, float finalMin, float finalMax) {
            return RemapValue(value, 0, 1, finalMin, finalMax);
        }
    }
}
