using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    public class TreeGenerator : MonoBehaviour {
        [SerializeField] private NoiseSettings treeNoiseSettings;
        [SerializeField] private DomainWarping domainWarping;

        public TreeData GenerateTreeData(World world, ChunkData chunkData) {
            var treeData = new TreeData();
            float[,] treeNoise = GenerateTreeNoise(chunkData, world.Seed);
            treeData.TreePositions = DataProcessing.FindLocalMaxima(treeNoise);
            return treeData;
        }

        private float[,] GenerateTreeNoise(ChunkData chunkData, int seed) {
            float[,] noiseMax = new float[chunkData.ChunkSize, chunkData.ChunkSize];
            int xMax = chunkData.WorldPosition.x + chunkData.ChunkSize;
            int xMin = chunkData.WorldPosition.x;
            int zMax = chunkData.WorldPosition.z + chunkData.ChunkSize;
            int zMin = chunkData.WorldPosition.z;

            int xIndex = 0;
            int zIndex = 0;
            for (int x = xMin; x < xMax; x++) {
                for (int z = zMin; z < zMax; z++) {
                    noiseMax[xIndex, zIndex] = domainWarping.GenerateDomainNoise(x, z, treeNoiseSettings, seed);
                    zIndex++;
                }
                xIndex++;
                zIndex = 0;
            }

            return noiseMax;
        }
    }
}
