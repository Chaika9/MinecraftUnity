using System;
using Minecraft.World.Biome;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World {
    public class TerrainGenerator : MonoBehaviour {
        [Header("Terrain Settings")]
        [SerializeField] private TerrainType terrainType = TerrainType.Procedural;

        [SerializeField] private BiomeGenerator defaultBiomeGenerator;
        [SerializeField] private int waterThreshold = 34;

        public TerrainType TerrainType {
            get => terrainType;
            set => terrainType = value;
        }

        public int WaterThreshold => waterThreshold;

        /// <summary>
        ///     Generates the chunk data.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="chunkData">The chunk data.</param>
        /// <returns>The updated chunk data.</returns>
        public ChunkData GenerateChunkData(World world, ChunkData chunkData) {
            return terrainType switch {
                TerrainType.Procedural => ProceduralGeneration(world, chunkData),
                _ => throw new Exception("Terrain type not implemented.")
            };
        }

        public void GenerateAdditionalChunkData(World world, ChunkData chunkData) {
            for (int x = 0; x < world.ChunkSize; x++) {
                for (int z = 0; z < world.ChunkSize; z++) {
                    chunkData = defaultBiomeGenerator.GenerateAdditionalChunkData(world, chunkData, x, z);
                }
            }
        }

        /// <summary>
        ///     Procedural generation.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="chunkData">The chunk data.</param>
        /// <returns>The updated chunk data.</returns>
        private ChunkData ProceduralGeneration(World world, ChunkData chunkData) {
            for (int x = 0; x < world.ChunkSize; x++) {
                for (int z = 0; z < world.ChunkSize; z++) {
                    chunkData = defaultBiomeGenerator.GenerateChunkData(world, chunkData, x, z);
                }
            }
            return chunkData;
        }
    }
}
