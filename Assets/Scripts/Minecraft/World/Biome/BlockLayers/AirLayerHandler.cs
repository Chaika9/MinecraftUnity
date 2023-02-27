using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    public class AirLayerHandler : BlockLayerHandler {
        public override bool TryHandling(World world, ChunkData chunkData, Vector3Int blockPosition, int groundPosition) {
            if (blockPosition.y > groundPosition) {
                ChunkHelper.SetBlock(chunkData, blockPosition, "air");
                return true;
            }
            return false;
        }
    }
}
