using System;
using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    [RequireComponent(typeof(DomainWarping))]
    public class StoneLayerHandler : BlockLayerHandler {
        [Range(0, 1)]
        [SerializeField] private float stoneThreshold = 0.5f;

        [SerializeField] private NoiseSettings stoneNoiseSettings;

        private DomainWarping domainWarping;

        private void Awake() {
            if (stoneNoiseSettings == null) {
                throw new Exception("Stone noise settings is null");
            }
            domainWarping = GetComponent<DomainWarping>();
        }

        public override bool TryHandling(World world, ChunkData chunkData, Vector3Int blockPosition, int groundPosition) {
            if (chunkData.WorldPosition.y > groundPosition) {
                return false;
            }

            float stoneNoise = domainWarping.GenerateDomainNoise(chunkData.WorldPosition.x + blockPosition.x,
                chunkData.WorldPosition.z + blockPosition.z, stoneNoiseSettings, world.Seed);

            int endPosition = groundPosition;
            if (chunkData.WorldPosition.y < 0) {
                endPosition = chunkData.WorldPosition.y + chunkData.ChunkHeight;
            }

            if (stoneNoise > stoneThreshold) {
                for (int y = chunkData.WorldPosition.y; y <= endPosition; y++) {
                    ChunkHelper.SetBlock(chunkData, new Vector3Int(blockPosition.x, y, blockPosition.z), "stone");
                }
                return true;
            }
            return false;
        }
    }
}
