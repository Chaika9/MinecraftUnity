using Minecraft.World.Chunk;
using UnityEngine;

namespace Minecraft.World.Biome {
    public class UndergroundLayerHandler : BlockLayerHandler {
        [SerializeField] private string undergroundBlockId = "stone";

        public override bool TryHandling(World world, ChunkData chunkData, Vector3Int blockPosition, int groundPosition) {
            if (blockPosition.y == -200) {
                var localPosition = new Vector3Int(blockPosition.x, blockPosition.y - chunkData.WorldPosition.y, blockPosition.z);
                ChunkHelper.SetBlock(chunkData, localPosition, "bedrock");
                return true;
            }
            if (blockPosition.y < groundPosition) {
                var localPosition = new Vector3Int(blockPosition.x, blockPosition.y - chunkData.WorldPosition.y, blockPosition.z);
                ChunkHelper.SetBlock(chunkData, localPosition, undergroundBlockId);
                return true;
            }
            return false;
        }
    }
}
