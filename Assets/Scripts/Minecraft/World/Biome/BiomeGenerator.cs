using System.Collections.Generic;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    [RequireComponent(typeof(DomainWarping), typeof(TreeGenerator))]
    public class BiomeGenerator : MonoBehaviour {
        [Header("Biome Settings")]
        [SerializeField] private NoiseSettings biomeNoiseSettings;

        [SerializeField] private List<BlockLayerHandler> blockLayerHandlers;
        [SerializeField] private List<BlockLayerHandler> additionalBlockLayerHandlers;

        private DomainWarping domainWarping;
        private TreeGenerator treeGenerator;

        private void Awake() {
            domainWarping = GetComponent<DomainWarping>();
            treeGenerator = GetComponent<TreeGenerator>();
        }

        public ChunkData GenerateChunkData(World world, ChunkData chunkData, int x, int z) {
            // Get the ground position (surface height)
            int groundPosition = GetGroundPosition(chunkData.WorldPosition.x + x, chunkData.WorldPosition.z + z, chunkData.ChunkHeight, world.Seed);
            groundPosition += Mathf.Abs(chunkData.WorldPosition.y);
            
            // Generate the tree data for the chunk
            chunkData.TreeData = treeGenerator.GenerateTreeData(world, chunkData);

            for (int y = chunkData.WorldPosition.y; y < chunkData.WorldPosition.y + world.ChunkHeight; y++) {
                foreach (BlockLayerHandler layer in blockLayerHandlers) {
                    var blockPosition = new Vector3Int(x, y, z);
                    if (layer.TryHandling(world, chunkData, blockPosition, groundPosition)) {
                        break;
                    }
                }
            }
            return chunkData;
        }
        
        public ChunkData GenerateAdditionalChunkData(World world, ChunkData chunkData, int x, int z) {
            // Get the ground position (surface height)
            int groundPosition = GetGroundPosition(chunkData.WorldPosition.x + x, chunkData.WorldPosition.z + z, chunkData.ChunkHeight, world.Seed);
            groundPosition += Mathf.Abs(chunkData.WorldPosition.y);
            
            foreach (BlockLayerHandler layer in additionalBlockLayerHandlers) {
                var blockPosition = new Vector3Int(x, chunkData.WorldPosition.y, z);
                layer.TryHandling(world, chunkData, blockPosition, groundPosition);
            }
            return chunkData;
        }

        private int GetGroundPosition(int x, int z, int chunkHeight, int seed) {
            float terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings, seed);
            terrainHeight = NoiseHelper.Redistribute(terrainHeight, biomeNoiseSettings);
            return (int)NoiseHelper.RemapValue(terrainHeight, 0, chunkHeight);
        }
    }
}
