using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    public class WaterLayerHandler : BlockLayerHandler {
        public override bool TryHandling(World world, ChunkData chunkData, Vector3Int blockPosition, int groundPosition) {
            if (blockPosition.y > groundPosition && blockPosition.y <= world.TerrainGenerator.WaterThreshold) {
                if (blockPosition.y == groundPosition + 1) {
                    ChunkHelper.SetBlock(chunkData, blockPosition, "sand");
                    return true;
                }

                ChunkHelper.SetBlock(chunkData, blockPosition, "water");
                return true;
            }

            if (blockPosition.y <= world.TerrainGenerator.WaterThreshold && blockPosition.y >= groundPosition - 4) {
                ChunkHelper.SetBlock(chunkData, blockPosition, "sand");
                return true;
            }
            return false;
        }
    }
}
