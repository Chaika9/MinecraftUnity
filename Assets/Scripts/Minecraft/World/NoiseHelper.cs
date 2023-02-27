using UnityEngine;

namespace Minecraft.World {
    public static class NoiseHelper {
        public static float GetNoise(float x, float z, float offset, float scale) {
            return Mathf.PerlinNoise((x + offset) * scale, (z + offset) * scale);
        }

        public static float OctavePerlin(float x, float z, NoiseSettings noiseSettings, int seed) {
            // Apply scale
            x *= noiseSettings.scale;
            z *= noiseSettings.scale;
            x += noiseSettings.scale;
            z += noiseSettings.scale;

            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0; // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < noiseSettings.octaves; i++) {
                total += Mathf.PerlinNoise((x + noiseSettings.offset.x + seed) * frequency,
                    (z + noiseSettings.offset.y + seed) * frequency) * amplitude;

                maxValue += amplitude;

                amplitude *= noiseSettings.persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }

        public static float Redistribute(float value, NoiseSettings noiseSettings) {
            return Mathf.Pow(value * noiseSettings.redistribution, noiseSettings.exponent);
        }

        public static float RemapValue(float value, float initialMin, float initialMax, float finalMin, float finalMax) {
            return finalMin + (value - initialMin) * (finalMax - finalMin) / (initialMax - initialMin);
        }

        public static float RemapValue(float value, float finalMin, float finalMax) {
            return RemapValue(value, 0, 1, finalMin, finalMax);
        }
    }
}
